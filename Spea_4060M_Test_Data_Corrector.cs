using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

// This program aids correcting erroneous Spea 4060M test data.
// - Tailored to Certification Data Collection (CDCol) version 6, the highest version both ABT & Test Coach can generate.
// - CDCol 7 is the most current, but Test Coach hasn't installed the more recent versions of Spea Leonardo, so doesn't have access to CDCol 7.
// - No compelling improvements between CDCol 6 & 7, so no worries remaining with CDCol 6.
// - 5/5/22, P. Smelt
namespace Spea_4060M_Test_Data_Corrector {
    internal struct TestLimit {
        internal String value, prefix, unit;
        internal TestLimit(String value, String prefix, String unit) { this.value = value; this.prefix = prefix; this.unit = unit; }
    }

    public partial class Spea_4060M_Test_Data_Corrector : Form {
        internal FileStream fileStream = null;
        internal String testDataDirectory = ConfigurationManager.AppSettings["RootDirectoryTDR"].Trim();

        public Spea_4060M_Test_Data_Corrector() { this.InitializeComponent(); }
        private void BtnCorrectData_Click(Object sender, EventArgs e) {
            // Enabled/disabled states of btnCorrectData & btnDoNotCorrectData are used as flags to method CorrectTestFailure.
            this.BtnDoNotCorrectData.Enabled = false;
        }

        private void BtnDoNotCorrectData_Click(Object sender, EventArgs e) {
            // Enabled/disabled states of btnCorrectData & btnDoNotCorrectData are used as flags to method CorrectTestFailure.
            this.BtnCorrectData.Enabled = false;
        }

        private void BtnOpenTestData_Click(Object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = testDataDirectory;
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                openFileDialog.Title = "Select a Spea 4060M Test Data file";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    testDataDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    this.ProcessTestFile(openFileDialog.FileName);
                }
            }
            this.txtTestData.Text = "";
        }

        private void Form_Closing(Object sender, FormClosingEventArgs e) { this.fileStream?.Close(); }

        private void ProcessTestFile(String fileName) {
            this.Text = fileName;
            String[] testData = File.ReadAllLines(fileName);
            Int32 failures = (from test in testData where test.Contains("FAIL") select test).Count();
            if (failures == 0) {
                MessageBox.Show("Search String 'FAIL' not found in this test data file.", "'FAIL' not found.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<String> boardResults = (from test in testData where test.StartsWith("BOARDRESULT") select test).ToList();
            if (boardResults[0].Contains("INTERRUPTED")) {
                MessageBox.Show("A BOARDRESULT was INTERRUPTED, can't correct incomplete test data.", "Yikes!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.BtnOpenTestData.Enabled = false;
            try {
                this.fileStream = File.Open(fileName, FileMode.Open);
                for (Int32 i = 0; i < testData.Length; i++) testData[i] = this.ProcessTestLine(testData[i]);
            } finally {
                this.fileStream.Close();
                this.fileStream.Dispose();
                this.fileStream = null;
            }
            this.txtTestData.Text = "";
                String tempFileName = Path.GetTempFileName();
                File.WriteAllLines(tempFileName, testData);
                File.Delete(fileName);
                File.Move(tempFileName, fileName);
                failures = (from test in testData where test.Contains("FAIL") select test).Count();
                if (failures != 0) {
                    MessageBox.Show(String.Format("Search String 'FAIL' still found in this test data file.{0}{0}" +
                                                  "Cannot rename file with '_P.txt' suffix.", Environment.NewLine), "'FAIL' still found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } else {
                    MessageBox.Show(String.Format("Search String 'FAIL' no longer found in this test data file.{0}{0}" +
                                                  "Renaming file with '_P.txt' suffix for Pass instead of '_F.txt'.", Environment.NewLine), "'FAIL' no longer found.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    File.Move(fileName, fileName.Replace("_F.txt", "_P.txt"));
                }
            this.Text = "Spea 4060M Test Data Corrector";
            this.BtnOpenTestData.Enabled = true;
        }

        private String ProcessTestLine(String testLine) {
            // CDCol 6's 14 field numbers, names & sample data shown below, with tabs inserted between semi-colon field delimiters for readability.
            // 0           1        2           3           4           5                   6           7           8                   9               10              11          12              13
            // BlockType;  Site;   TaskName;    TaskNumber; TestTask;   DiagnosticMessage;  PartNumber; TestResult; TestMeasurement;    LowLimit;       HighLimit;      TestUnits;  TestPoints;     UniqueID;
            // ANL;        1;       R116;       46;         1;          RESR116 10K 1 %;    123;        PASS;       1.001001e+04;       9.700000e+03;   1.030000e+04;   ohm;        579 1488; 9270  (N/A);
            String[] testSplit = testLine.Split(';');
            switch (testSplit[0].ToUpper()) {
                case "ANL": case "ESCAN": case "FUNC": case "JSCAN": case "OPT":
                    // Field 7=Result, 8=Measure, 9=Thr Min, 10=Thr Max & 11=Measure Type.
                    if (testSplit[7].Contains("FAIL") && this.CorrectTestFailure(testLine: testLine, testSplit: testSplit, startField: 7, fieldCount: 2)) {
                        Double thrMin = Double.Parse(testSplit[9]), thrMax = Double.Parse(testSplit[10]);
                        String measureType = testSplit[11], testUnits;
                        TestLimit lowLimit, highLimit;
                        if (testSplit[9].Equals("-1.000000e+10")) {
                            // Spea's representation of -∞, which means no low limit.
                            lowLimit.value = "NA"; lowLimit.prefix = ""; lowLimit.unit = "";
                            highLimit = this.ConvertLimit(thrMax, measureType);
                            testUnits = highLimit.prefix + highLimit.unit;
                        } else if (testSplit[10].Equals("1.000000e+10")) {
                            // Spea's representation of +∞, which means no high limit.
                            highLimit.value = "NA"; highLimit.prefix = ""; highLimit.unit = "";
                            lowLimit = this.ConvertLimit(thrMin, measureType);
                            testUnits = lowLimit.prefix + lowLimit.unit;
                        } else {
                            lowLimit = this.ConvertLimit(thrMin, measureType);
                            highLimit = this.ConvertLimit(thrMax, measureType);
                            if (lowLimit.prefix != highLimit.prefix) {
                                // Using different Metric Prefixes for limits; example, 850.000000 nanoF to 1.150000 microF.  Thus need to normalize both to same Metric Prefix.
                                // Choose lowLimit's, to prevent entering userInput <= 1; example, enter 850 to 1150 nanoF versus 0.850 to 1.15 microF.
                                Int32 expDiff = this.MetricPrefixToExponent(highLimit.prefix) - this.MetricPrefixToExponent(lowLimit.prefix);
                                // For 850.000000 nanoF to 1.150000 microF, expDiff would be -6 - -9 = 3, thus 10^3 = 1000 = a thousand-fold difference.
                                // Multiply highLimit's mantissa by 10^expDiff to normalize Metric Prefixes.
                                Double mantissa = Double.Parse(highLimit.value) * Math.Pow(10, expDiff);
                                highLimit.value = String.Format(mantissa.ToString(), "##0.000000");
                                highLimit.prefix = lowLimit.prefix;
                            }
                            testUnits = lowLimit.prefix + lowLimit.unit;
                        }
                        String q = String.Format("Low limit:  {0} {1}{2}" +
                                                 "High limit: {3} {1}{2}" +
                                                 "- Enter measured value from Spea 4060M 'Test Select' in {1}, without prefix, unit or scientific notation.{2}" +
                                                 "- Enter nothing, cancel or close to skip correction.", lowLimit.value, testUnits, Environment.NewLine, highLimit.value);
                    PromptUser:
                        String a = Microsoft.VisualBasic.Interaction.InputBox(q, "Enter measured value.", "");
                        if (a != "") {
                            if (Double.TryParse(a, out Double d)) {
                                if (lowLimit.value == "NA") {
                                    if (d <= Double.Parse(highLimit.value)) this.PassTestResult(d, highLimit.prefix, ref testSplit);
                                    else {
                                        MessageBox.Show("Input must be less than or equal to High limit.", "Oopsie!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        goto PromptUser;
                                    }
                                } else if (highLimit.value == "NA") {
                                    if (Double.Parse(lowLimit.value) <= d) this.PassTestResult(d, lowLimit.prefix, ref testSplit);
                                    else {
                                        MessageBox.Show("Input must be greater than or equal to Low limit.", "Oopsie!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        goto PromptUser;
                                    }
                                } else {
                                    if (Double.Parse(lowLimit.value) <= d && d <= Double.Parse(highLimit.value)) this.PassTestResult(d, lowLimit.prefix, ref testSplit);
                                    else {
                                        MessageBox.Show("Input must be between Low & High limits.", "Oopsie!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        goto PromptUser;
                                    }
                                }
                            } else {
                                MessageBox.Show("Non numeric input, try again.", "Oopsie!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                goto PromptUser;
                            }
                        }
                    }
                    break;
                case "BOARDRESULT":
                    // Field 2,3,4...n=Result(s).
                    for (Int32 i = 1; i < testSplit.Length; i++) {
                        if (testSplit[i].Contains("FAIL") && this.CorrectTestFailure(testLine: testLine, testSplit: testSplit, startField: i, fieldCount: 1)) testSplit[i] = "PASS";
                        testLine = String.Join(";", testSplit);
                    }
                    break;
                case "DIG":
                    // Field 6=Result.  No Measure, Thr Min, Thr Max or Measure Type fields.
                    if (testSplit[5].Contains("FAIL") && this.CorrectTestFailure(testLine: testLine, testSplit: testSplit, startField: 5, fieldCount: 1)) testSplit[5] = "PASS";
                    break;
                case "END":
                    // Field 2=Result.
                    if (testSplit[1].Contains("FAIL") && this.CorrectTestFailure(testLine: testLine, testSplit: testSplit, startField: 1, fieldCount: 1)) testSplit[1] = "PASS";
                    break;
                case "OBP":
                    // Field 8=Result.  OBP's documentation references but doesn't use Measure, Thr Min, Thr Max or Measure Type fields.
                    if (testSplit[7].Contains("FAIL") && this.CorrectTestFailure(testLine: testLine, testSplit: testSplit, startField: 7, fieldCount: 1)) testSplit[7] = "PASS";
                    break;
                case "ANLAUTO": case "ASCC": case "BSC": case "CSCAN":
                    // Couldn't find sufficient documentation for "ANLAUTO", "ASCC", "BSC" or "CSCAN" to correct correctly.
                    MessageBox.Show(String.Format("Cannot correct following test record types:{0}{0}ANLAUTO{0}ASCC{0}BSC{0}CSCAN{0}{0}; correct manually.  Data not saved, exiting!", Environment.NewLine), "Data not saved!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    break;
                default:
                    break;
            }
            return String.Join(";", testSplit);
        }

        private Boolean CorrectTestFailure(String testLine, String[] testSplit, Int32 startField, Int32 fieldCount) {
            this.txtTestData.Text = testLine;
            String txtFind = String.Join(";", testSplit, startField, fieldCount);
            String startTxtFind = String.Join(";", testSplit, 0, startField);
            Int32 startFind = startTxtFind.Length - 1;
            String endTxtFind = String.Join(";", testSplit, 0, startField + fieldCount);
            Int32 endFind = endTxtFind.Length + 1;
            this.txtTestData.Find(txtFind, startFind, endFind, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
            this.txtTestData.SelectionColor = Color.Red;
            this.txtTestData.SelectionFont = new Font(this.txtTestData.SelectionFont.FontFamily, this.txtTestData.SelectionFont.Size + 2.0F, FontStyle.Bold);
            this.BtnCorrectData.Enabled = this.BtnDoNotCorrectData.Enabled = true;
            while (this.BtnCorrectData.Enabled && this.BtnDoNotCorrectData.Enabled) Application.DoEvents();
            Boolean CorrectTestFailure = this.BtnCorrectData.Enabled;
            this.BtnCorrectData.Enabled = this.BtnDoNotCorrectData.Enabled = false;
            return CorrectTestFailure;
        }

        private String SciNotToMetricPrefix(Double dbl) {
            (Double, Int32) manExp = this.GetMantissaAndExponent(dbl);
            (Int32, String) preExpPre = this.GetPrefixedExponentAndPrefix(dbl);
            Double mantissa = manExp.Item1 * Math.Pow(10, Convert.ToDouble(preExpPre.Item1));
            return mantissa.ToString("##0.000000") + " " + preExpPre.Item2;
        }

        private (Double, Int32) GetMantissaAndExponent(Double dbl) {
            String s = dbl.ToString("+0.000000e+00;-0.000000e+00");
            // Force inclusion of + sign, exponential sgn & all digits so s.Length = 13, always, without exception.
            return (Double.Parse(s.Substring(0, 9)), Int32.Parse(s.Substring(10)));
        }

        private (Int32, String) GetPrefixedExponentAndPrefix(Double SciNotNumber) {
            (Double, Int32) manExp = this.GetMantissaAndExponent(SciNotNumber);
            switch (manExp.Item2) {
                case Int32 e when -24 <= e && e <= -22: return (manExp.Item2 + 24, "yocto");
                case Int32 e when -21 <= e && e <= -19: return (manExp.Item2 + 21, "zepto");
                case Int32 e when -18 <= e && e <= -16: return (manExp.Item2 + 18, "atto");
                case Int32 e when -15 <= e && e <= -13: return (manExp.Item2 + 15, "femto");
                case Int32 e when -12 <= e && e <= -10: return (manExp.Item2 + 12, "pico");
                case Int32 e when -9 <= e && e <= -7:   return (manExp.Item2 + 9, "nano");
                case Int32 e when -6 <= e && e <= -4:   return (manExp.Item2 + 6, "micro");
                case Int32 e when -3 <= e && e <= -1:   return (manExp.Item2 + 3, "milli");
                case Int32 e when 0 <= e && e <= 2:     return (manExp.Item2 + 0, "");
                case Int32 e when 3 <= e && e <= 5:     return (manExp.Item2 - 3, "kilo");
                case Int32 e when 6 <= e && e <= 8:     return (manExp.Item2 - 6, "mega");
                case Int32 e when 9 <= e && e <= 11:    return (manExp.Item2 - 9, "giga");
                case Int32 e when 12 <= e && e <= 14:   return (manExp.Item2 - 12, "tera");
                case Int32 e when 15 <= e && e <= 17:   return (manExp.Item2 - 15, "peta");
                case Int32 e when 18 <= e && e <= 20:   return (manExp.Item2 - 18, "exa");
                case Int32 e when 21 <= e && e <= 23:   return (manExp.Item2 - 21, "zetta");
                case Int32 e when 24 <= e && e <= 26:   return (manExp.Item2 - 24, "yotta");
                default:                                return (0, "");
            }
        }

        private TestLimit ConvertLimit(Double testLimit, String testUnit) {
            String[] vp = this.SciNotToMetricPrefix(testLimit).Split(' ');
            return new TestLimit(value: vp[0], prefix: vp[1], unit: testUnit.ToUpper());
        }

        private Int32 MetricPrefixToExponent(String prefix) {
            switch (prefix) {
                case "yocto":   return -24;
                case "zepto":   return -21;
                case "atto":    return -18;
                case "femto":   return -15;
                case "pico":    return -12;
                case "nano":    return -9;
                case "micro":   return -6;
                case "milli":   return -3;
                case "":        return 0;
                case "kilo":    return 3;
                case "mega":    return 6;
                case "giga":    return 9;
                case "tera":    return 12;
                case "peta":    return 15;
                case "exa":     return 18;
                case "zetta":   return 21;
                case "yotta":   return 24;
                default:        return 0;
            }
        }

        private void PassTestResult (Double d, String prefix, ref String[] testSplit) {
            Double f = d * Math.Pow(10, this.MetricPrefixToExponent(prefix));
            testSplit[7] = "PASS";
            if  (testSplit[0].ToUpper().Equals("OPT")) testSplit[8] = f.ToString("#00");            // OPT TestMeasurement range is 70 to 100, requires non-Scientific Notation.
            else                                       testSplit[8] = f.ToString("0.000000e+00");   // ANL, ESCAN, FUNC & JSCAN TestMeasurement requires Scientific Notation.
        }
    }
}
