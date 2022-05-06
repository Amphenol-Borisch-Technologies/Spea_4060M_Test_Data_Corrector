using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
// This program aids correction of Spea 4060M test data.
// - Tailored to Certification Data Collection (CDCol) version 6, the highest version both ABT & Test Coach can generate.
// - CDCol 7 is the most current, but Test Coach hasn't installed the more recent versions of Spea Leonardo, so doesn't have access to CDCol 7.
// - No compelling improvements between CDCol 6 & 7, so no worries remaining with CDCol 6.
// - 5/5/22, P. Smelt
namespace Spea_4060M_Test_Data_Corrector {
    internal struct TestLimit {
        internal string value, prefix, unit;
        internal TestLimit(string value, string prefix, string unit) { this.value = value; this.prefix = prefix; this.unit = unit; }
    }

    public partial class Spea_4060M_Test_Data_Corrector : Form {
        public Spea_4060M_Test_Data_Corrector() { InitializeComponent(); }

        private void btnCorrectData_Click(object sender, EventArgs e) {
            string line = this.txtTestData.Text;
            string[] lineSplit = line.Split(';');
            string measureType;
            double measure, thrMin, thrMax;
            measure = double.Parse(lineSplit[8]);
            thrMin = double.Parse(lineSplit[9]);
            thrMax = double.Parse(lineSplit[10]);
            measureType = lineSplit[11];
            // Prompt operator for corrections.
            // If corrections valid and operator didn't cancel, save corrected data.
            this.btnCorrectData.Enabled = this.btnDoNotCorrectData.Enabled = false;
        }

        private void btnDoNotCorrectData_Click(object sender, EventArgs e) {
            this.btnCorrectData.Enabled = this.btnDoNotCorrectData.Enabled = false;
        }

        private void btnOpenTestData_Click(object sender, EventArgs e) {
             using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = @"\\spea4060 - pc\c$\TDR";
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                openFileDialog.Title = "Select a Spea 4060M Test Data file";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK) processTestFile(openFileDialog.FileName);
            }
            this.txtTestData.Text = "";
        }

        private void processTestFile(string fileName) {
            this.btnOpenTestData.Enabled = false;
            string[] testData = File.ReadAllLines(fileName);
            using (FileStream fileStream = File.Open(fileName, FileMode.Open)) {
                // Lock file so no one else edits it concurrently.  Also, will need to write to it after corrections completed.
                foreach (string testLine in testData) processTestLine(testLine);
                // Save Test Data.
            }
            MessageBox.Show("No (more) failing test data to correct.", "No (more) failures", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.btnOpenTestData.Enabled = true;
        }

        private void processTestLine(string testLine) {
            string[] testSplit = testLine.Split(';');
            switch (testSplit[0].ToUpper()) {
                case "ANL":  case "ESCAN":  case "FUNC":  case "JSCAN":  case "OPT":
                    // Field 8=Result, 9=Measure, 10=Thr Min, 11=Thr Max & 12=Measure Type.
                    // Couldn't find sufficient documentation for "JSCAN", but guessed it'd be identical to "ESCAN".
                    if (testSplit[7].Contains("FAIL")) processTestFailure(testData: testLine, testResult: testSplit[7], testMeasurement: testSplit[8]);
                    break;
                case "BOARDRESULT":
                    // Field 2,3,4...n=Result(s).
                    for (int i = 1; i < testSplit.Length; i++)
                        if (testSplit[i].Contains("FAIL")) processTestFailure(testData: testLine, testResult: testSplit[i]);
                    break;
                case "DIG":
                    // Field 6=Result.  No Measure, Thr Min, Thr Max or Measure Type fields.
                    if (testSplit[5].Contains("FAIL")) processTestFailure(testData: testLine, testResult: testSplit[5]);
                    break;
                case "END":
                    // Field 2=Result.
                    if (testSplit[1].Contains("FAIL")) processTestFailure(testData: testLine, testResult: testSplit[1]);
                    break;
                case "OBP":
                    // Field 8=Result, 9=Measure, 10=Thr Min, 11=Thr Max & 12=Measure Type.
                    // But, OBP doesn't use Measure, Thr Min, Thr Max or Measure Type fields.
                    if (testSplit[7].Contains("FAIL")) processTestFailure(testData: testLine, testResult: testSplit[7]);
                    break;
                case "ANLAUTO":  case "ASCC":  case "BSC":  case "CSCAN":
                    // Couldn't find sufficient documentation for "ANLAUTO", "ASCC", "BSC" or "CSCAN" to handle correctly.
                    MessageBox.Show(string.Format("Cannot correct following test record types:{0}{0}ANLAUTO{0}ASCC{0}BSC{0}CSCAN.{0}{0}They must be corrected manually.", Environment.NewLine), "Must correct manually!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                default:
                    break;
            }
        }
        private void processTestFailure(string testData, string testResult, string testMeasurement = "") {
            this.txtTestData.Text = testData;
            this.txtTestData.Find(testResult);
            this.txtTestData.SelectionColor = Color.Red;
            this.txtTestData.SelectionFont = new Font(this.txtTestData.SelectionFont.FontFamily, this.txtTestData.SelectionFont.Size + 2.0F, FontStyle.Bold);
            if (testMeasurement != "") {
                this.txtTestData.Find(testMeasurement);
                this.txtTestData.SelectionColor = Color.Red;
                this.txtTestData.SelectionFont = new Font(this.txtTestData.SelectionFont.FontFamily, this.txtTestData.SelectionFont.Size + 2.0F, FontStyle.Bold);
            }
            this.btnCorrectData.Enabled = this.btnDoNotCorrectData.Enabled = true;
            while (this.btnCorrectData.Enabled && this.btnDoNotCorrectData.Enabled) Application.DoEvents();
        }

        private (int, string) GetPrefixedExponentAndPrefix(double SciNotNumber) {
            (double, int) manExp = GetMantissaAndExponent(SciNotNumber);
            switch (manExp.Item2) {
                case int e when -24 <= e && e <= -22: return (manExp.Item2 + 24, "yocto");
                case int e when -21 <= e && e <= -19: return (manExp.Item2 + 21, "zepto");
                case int e when -18 <= e && e <= -16: return (manExp.Item2 + 18, "atto");
                case int e when -15 <= e && e <= -13: return (manExp.Item2 + 15, "femto");
                case int e when -12 <= e && e <= -10: return (manExp.Item2 + 12, "pico");
                case int e when  -9 <= e && e <=  -7: return (manExp.Item2 +  9, "nano");
                case int e when  -6 <= e && e <=  -4: return (manExp.Item2 +  6, "micro");
                case int e when  -3 <= e && e <=  -1: return (manExp.Item2 +  3, "milli");
                case int e when   0 <= e && e <=   2: return (manExp.Item2 +  0, "");
                case int e when   3 <= e && e <=   5: return (manExp.Item2 -  3, "kilo");
                case int e when   6 <= e && e <=   8: return (manExp.Item2 -  6, "mega");
                case int e when   9 <= e && e <=  11: return (manExp.Item2 -  9, "giga");
                case int e when  12 <= e && e <=  14: return (manExp.Item2 - 12, "tera");
                case int e when  15 <= e && e <=  17: return (manExp.Item2 - 15, "peta");
                case int e when  18 <= e && e <=  20: return (manExp.Item2 - 18, "exa");
                case int e when  21 <= e && e <=  23: return (manExp.Item2 - 21, "zetta");
                case int e when  24 <= e && e <=  26: return (manExp.Item2 - 24, "yotta");
                default: return (0,"");
            }
        }

        private (double, int) GetMantissaAndExponent(double dbl) {
            string s = dbl.ToString("+#.######E+00;-#.######E+00"); // Force inclusion of + sign so s.Length = 13, always.
            return (double.Parse(s.Substring(0, 9)), int.Parse(s.Substring(10)));
        }

        private string SciNotToMetricPrefix(double dbl) {
            (double, int) manExp = GetMantissaAndExponent(dbl);
            (int, string) pExpPre = GetPrefixedExponentAndPrefix(dbl);
            double mantissa = manExp.Item1 * Math.Pow(10, Convert.ToDouble(pExpPre.Item1));
            mantissa = Math.Round(mantissa, 6, MidpointRounding.AwayFromZero);
            return mantissa.ToString("##0.000000") + " " + pExpPre.Item2;
        }


    }
}
