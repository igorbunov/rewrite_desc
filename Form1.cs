using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Win32;

namespace Rewrite
{
    public partial class Form1 : Form
    {
        public string plan = "";
        public List<Key> words = new List<Key>();
        public Dictionary<string, string> synonims = new Dictionary<string, string>();
        HashSet<string> allowedWords = new HashSet<string>();
        public string currentFile = "";
        public System.Windows.Forms.Timer highlightTimer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer workTimeTimer = new System.Windows.Forms.Timer();
        public XmlDocument currentDocumentAutosave = null;
        public string autosaveFilename = "rewrite_autosave.rwt";
        public List<History> history = new List<History>();
        public List<Key> tempWords = new List<Key>();
        RichTextBox tempRichText = new RichTextBox();
        public string curProgramCode = "";
        public int curProgramRunCount = 0;
        public bool dontShowMatchedWords = false;
        public bool dontShowMatchedKeysWords = false;
        public bool dontShowWrongWords = false;
        private int resourceLoadder = 0;
        private StartForm startFrm = new StartForm();
        public int mainTimerTickCounter = 0;

        public Form1()
        {
            InitializeComponent();

            if (DoRegisterJob() == false)
            {
                this.Text = "Количество запусков программы закончилось. Выполните активацию!";
                panel2.Enabled = false;
                menuStrip1.Enabled = true;
                файлToolStripMenuItem.Enabled = false;
                копироватьToolStripMenuItem.Enabled = false;
                сайтыToolStripMenuItem.Enabled = false;
                планToolStripMenuItem.Enabled = false;
                ключевыеСловаToolStripMenuItem.Enabled = false;
                contextMenuAboutProgramMain.Enabled = true;
                таймерToolStripMenuItem.Enabled = false;

                Activation frm = new Activation();
                frm.SetValues(curProgramCode, 0);

                if (frm.ShowDialog() == DialogResult.Yes)
                {
                    doActivation(frm.txtActivationCode.Text.Trim());
                }
            }

            this.Opacity = 0;
            startFrm.Show();

            clearSearchBtn.Hide();
            searchLabel.Hide();

            TextBox.CheckForIllegalCrossThreadCalls = false;
            RichTextBox.CheckForIllegalCrossThreadCalls = false;
            стопТаймераToolStripMenuItem.Enabled = false;
            обнулитьВремяToolStripMenuItem.Enabled = false;
            highlightTimer.Interval = 3000;
            highlightTimer.Tick += new EventHandler(t_Tick);
            highlightTimer.Start();
            
            workTimeTimer.Interval = 60 * 1000;
            workTimeTimer.Tick += new EventHandler(workTimeTimer_Tick);

            System.Windows.Forms.Timer collector = new System.Windows.Forms.Timer();
            collector.Interval = 60 * 1000;
            collector.Tick += new EventHandler(collector_Tick);
            collector.Start();

            string isShowMatchWords = GetFromRegistry("RewriteProgramShoMatchedWords");
            string isShowMatchKeysWords = GetFromRegistry("RewriteProgramShoMatchedKeysWords");
            string isShowWrongWords = GetFromRegistry("RewriteProgramShoWrongWords");

            this.dontShowMatchedWords = (isShowMatchWords != null) ? bool.Parse(isShowMatchWords) : false;
            this.dontShowMatchedKeysWords = (isShowMatchKeysWords != null) ? bool.Parse(isShowMatchKeysWords) : false;
            this.dontShowWrongWords = (isShowWrongWords != null) ? bool.Parse(isShowWrongWords) : false;

            отключитьПодсветкуСовпаденийToolStripMenuItem.Checked = this.dontShowMatchedWords;
            отключитьПодсветкуКлючейToolStripMenuItem.Checked = this.dontShowMatchedKeysWords;
            отключитьПодсветкуОшибокToolStripMenuItem.Checked = this.dontShowWrongWords;

            ResourcesLoadder loadder = new ResourcesLoadder();
            loadder.doneLoadSynonimsEvent += new ResourcesLoadder.DoneLoadSyn(loadder_doneLoadSynonimsEvent);
            Thread th = new Thread(new ThreadStart(loadder.LoadSynonims));
            th.IsBackground = true;
            th.Start();

            loadder.doneLoadDictionaryEvent += new ResourcesLoadder.DoneLoadDict(loadder_doneLoadDictionaryEvent);
            Thread th2 = new Thread(new ThreadStart(loadder.LoadDictionary));
            th2.IsBackground = true;
            th2.Start();

            GC.Collect();
        }

        void loadder_doneLoadDictionaryEvent(HashSet<string> result)
        {
            allowedWords = result;
            result = null;
            resourceLoadder++;
            doneLoadResource();
        }

        void loadder_doneLoadSynonimsEvent(Dictionary<string, string> result)
        {
            synonims = result;
            result = null;
            resourceLoadder++;
            doneLoadResource();
        }

        void doneLoadResource()
        {
            if (resourceLoadder == 2)
            {
                GC.Collect();
                this.Opacity = 100;
                startFrm.Hide();
                this.WindowState = FormWindowState.Normal;
            }
        }

        void workTimeTimer_Tick(object sender, EventArgs e)
        {
            string[] arr = labelWorkTime.Text.Split(':');
            int hours = int.Parse(arr[0]);
            int minutes = int.Parse(arr[1]);

            if (++minutes >= 60)
            {
                hours++;
                minutes = 0;
            }

            string res = "";

            res = (hours < 10) ? "0" + hours.ToString() : hours.ToString();
            res += (minutes < 10) ? ":0" + minutes.ToString() : ":" + minutes.ToString();

            labelWorkTime.Text = res;
        }

        string GenerateProgramCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[32];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        bool DoRegisterJob()
        {
            curProgramCode = GetFromRegistry("RewriteProgramCode");

            if (curProgramCode == null)
            {
                curProgramCode = GenerateProgramCode();
                AddToRegistry("RewriteProgramCode", curProgramCode);
                curProgramRunCount = 10;
                AddToRegistry("RewriteProgramRunCounter", curProgramRunCount.ToString());
            }
            else
            {
                string runTimes = GetFromRegistry("RewriteProgramRunCounter");

                if (runTimes != null)
                {
                    curProgramRunCount = int.Parse(runTimes);
                    curProgramRunCount--;

                    if (curProgramRunCount < 0)
                    {
                        return false;
                    }
                    else
                    {
                        AddToRegistry("RewriteProgramRunCounter", curProgramRunCount.ToString());
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public string GetFromRegistry(string name)
        {
            RegistryKey reg;

            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            object res = reg.GetValue(name, null);

            if (res == null)
            {
                return null;
            }

            return res.ToString();
        }

        public bool AddToRegistry(string name, string value)
        {
            RegistryKey reg;

            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            string curValue = reg.GetValue(name, "").ToString();

            if (value == curValue && curValue != "")
            {
                return true;
            }

            try
            {
                reg.SetValue(name, value);
                reg.Flush();
                reg.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        void collector_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }

        void t_Tick(object sender, EventArgs e)
        {
            HighlightKeywords(richTextBox1);

            if (!IsEmptyDocument())
            {
                currentDocumentAutosave = GetCurDocument();

                if (mainTimerTickCounter++ % 3 == 0)
                {
                    Save(autosaveFilename, false, currentDocumentAutosave);
                }
            }
            
            unusedTxt.Text = "";

            for (int i = 0; i < this.words.Count; i++)
            {
                if (this.words[i].count > this.words[i].doneCount)
                {
                    unusedTxt.Text += this.words[i].text + "; ";
                }   
            }
            
            lblMemory.Text = "Память: " + Convert.ToString(Process.GetCurrentProcess().WorkingSet64 / 1048576) + " МБ";
        }

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "LockWindowUpdate", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LockWindow(IntPtr Handle);

        public void ScrollTo(int Position)
        {
            SetScrollPos((IntPtr)richTextBox1.Handle, 0x1, Position, true);
            PostMessage((IntPtr)richTextBox1.Handle, 0x115, 4 + 0x10000 * Position, 0);
        } 

        private void HighlightKeywords(RichTextBox focusedTextBox)
        {
            Stopwatch timer = new Stopwatch();

            LockWindow(this.Handle);
            int scrollPos = GetScrollPos(richTextBox1.Handle, 1);            
            bool isFocused = richTextBox1.Focused;
            int totalCount = 0;
            int found = 0;
            int startSel = richTextBox1.SelectionStart;
            int startLen = richTextBox1.SelectionLength;
            richTextBox1.HideSelection = true;

            tempRichText.Rtf = richTextBox1.Rtf;
            string originalText = tempRichText.Text;
            tempRichText.SelectionStart = 0;
            tempRichText.SelectionLength = originalText.Length;
            tempRichText.SelectionBackColor = Color.White;
            tempRichText.SelectionFont = new Font(tempRichText.SelectionFont, FontStyle.Regular);

            HashSet<string> endOfWordKeys = new HashSet<string>(new string[] { "\'", "\"", "^", "?", "!", ":", "(", "#", "№", ")", " ", ",", "—", "-", ".", ";", "\t", "\n", "\r\n" });

            if (!this.dontShowMatchedWords)
            {
                HashSet<string> matches = GetWordsToHighlight();
                
                foreach (string matchWord in matches)
                {
                    MatchCollection allIp = Regex.Matches(originalText, matchWord, RegexOptions.IgnoreCase);

                    for (int i = 0, findCnt = allIp.Count; i < findCnt; i++)
                    {
                        tempRichText.Select(allIp[i].Index, allIp[i].Length);
                        tempRichText.SelectionBackColor = Color.Yellow;
                    }

                    allIp = null;
                }

                matches = null;
            }

            if (tempWords.Count > 0)
            {
                timer.Start();

                List<FoundWords> foundWords = new List<FoundWords>();
                int tmpWordsCnt = tempWords.Count;

                for (int i = 0; i < tmpWordsCnt; i++)
                {
                    tempWords[i].doneCount = 0;
                    SetOriginalWordDoneCount(tempWords[i]);

                    totalCount += tempWords[i].count;
                    int curKeyFound = 0;

                    string matchKey = tempWords[i].text.Trim();
                    matchKey = matchKey.Replace(" ", @"([\s]|[,]|[,][\s]|[\s][,][\s]|[:]|[:][\s]|[\s][:][\s]|[-]|[\s][-][\s]|[\s][-]|[-][\s]|[\s]" + "[\\\"]|[\\\"]" + @"[\s])");
                    
                    Regex rx = new Regex(matchKey, RegexOptions.IgnoreCase);
                    MatchCollection allIp = rx.Matches(originalText);
                    rx = null;

                    for (int j = 0, allCnt = allIp.Count; j < allCnt; j++)
                    {
                        int endIndex = allIp[j].Index + allIp[j].Length;

                        if (allIp[j].Index >= 0 && endIndex <= originalText.Length)
                        {
                            string key1 = (allIp[j].Index == 0) ? "^" : originalText[allIp[j].Index - 1].ToString();
                            string key2 = (endIndex == originalText.Length) ? "^" : originalText[endIndex].ToString();

                            if (endOfWordKeys.Contains(key1) && endOfWordKeys.Contains(key2)) 
                            {
                                FoundWords curItem = new FoundWords() { length = allIp[j].Length, start = allIp[j].Index, text = allIp[j].Value };

                                if (!this.isFound(foundWords, curItem))
                                {
                                    if (++curKeyFound <= tempWords[i].count)
                                    {
                                        tempWords[i].doneCount++;
                                        SetOriginalWordDoneCount(tempWords[i]);
                                        found++;
                                    }

                                    if (!this.dontShowMatchedKeysWords)
                                    {
                                        tempRichText.SelectionStart = allIp[j].Index;
                                        tempRichText.SelectionLength = allIp[j].Length;
                                        tempRichText.SelectionBackColor = Color.Gray;    
                                    }

                                    foundWords.Add(curItem);
                                }
                            }

                            key1 = null;
                            key2 = null;
                        }
                    }
                    allIp = null;
                }
                foundWords = null;
            }

            string searchTextPrepared = searchTxt.Text.Trim().ToLower();
            int serachFoundCount = 0;

            if (searchTextPrepared != "")
            {
                MatchCollection searchMatches = Regex.Matches(originalText.ToLower(), searchTextPrepared);

                for (int i = 0, searchMatchesCnt = searchMatches.Count; i < searchMatchesCnt; i++)
                {
                    serachFoundCount++;
                    tempRichText.SelectionStart = searchMatches[i].Index;
                    tempRichText.SelectionLength = searchMatches[i].Length;
                    tempRichText.SelectionBackColor = Color.Red;
                }
                searchMatches = null;
            }

            searchLabel.Text = "Найдено " + serachFoundCount + " совпадений"; 
            keyLabel.Text = "Выполнение по ключевым фразам: " + found + " из " + totalCount;
            
            if (!this.dontShowWrongWords && allowedWords.Count > 0)
            {
                timer.Start();
                string[] text = GetWords(richTextBox1.Text);
                int cnt = text.Length;
                int counter = 0;
                string[] subres = new string[cnt];

                for (int i = 0; i < cnt; i++)
                {
                    if (text[i].Length > 1)
                    {
                        if (allowedWords.Contains(text[i]) == false)
                        {
                            subres[counter++] = text[i];
                        }
                    }
                }
                text = null;

                Array.Resize(ref subres, counter);

                foreach (string word in subres)
                {
                    MatchCollection allIp = Regex.Matches(originalText.ToLower(), word);

                    for (int j = 0, allCnt = allIp.Count; j < allCnt; j++)
                    {
                        int endIndex = allIp[j].Index + allIp[j].Length;

                        if (allIp[j].Index >= 0 && endIndex <= originalText.Length)
                        {
                            string key1 = (allIp[j].Index == 0) ? "^" : originalText[allIp[j].Index - 1].ToString();
                            string key2 = (endIndex == originalText.Length) ? "^" : originalText[endIndex].ToString();

                            if (endOfWordKeys.Contains(key1) && endOfWordKeys.Contains(key2)) 
                            {
                                tempRichText.SelectionStart = allIp[j].Index;
                                tempRichText.SelectionLength = allIp[j].Length;
                                tempRichText.SelectionFont = new Font(tempRichText.SelectionFont, FontStyle.Underline);
                            }
                        }
                    }
                    allIp = null;
                }

                if (subres.Length > 0)
                {
                    tempRichText.Rtf = tempRichText.Rtf.Replace(@"\ul\", @"\ulwave\").Replace(@"\ul ", @"\ulwave ");
                }
                subres = null;
            }

            tempRichText.SelectionStart = startSel;
            tempRichText.SelectionLength = startLen;
            richTextBox1.Rtf = tempRichText.Rtf;

            if (isFocused)
            {
                richTextBox1.Focus();
            }

            richTextBox1.SelectionStart = startSel;
            richTextBox1.SelectionLength = startLen;
            richTextBox1.SelectionBackColor = Color.White;
            ScrollTo(scrollPos);
            LockWindow(IntPtr.Zero);

            tempRichText.Text = "";
            originalText = null;
            endOfWordKeys = null;
        }

        private void SetOriginalWordDoneCount(Key word)
        {
            for (int i = 0; i < this.words.Count; i++)
            {
                if (this.words[i].text == word.text && this.words[i].count == word.count)
                {
                    this.words[i].doneCount = word.doneCount;
                }
            }
        }

        private bool isFound(List<FoundWords> foundWords, FoundWords key)
        {
            for (int i = 0; i < foundWords.Count; i++)
            {
                if (foundWords[i].text.IndexOf(key.text) != -1 
                    && foundWords[i].start <= key.start
                    && foundWords[i].start + foundWords[i].length >= key.start + key.length)
                {
                    return true;
                }
            }

            return false;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            calcSymvolCount();
        }

        public void CopyText()
        {
            AddToHistory();

            int startSel = richTextBox1.SelectionStart;
            int startLen = richTextBox1.SelectionLength;

            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = richTextBox1.Text.Length;

            richTextBox1.Copy();

            richTextBox1.Focus();
            richTextBox1.SelectionStart = startSel;
            richTextBox1.SelectionLength = startLen;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            CopyText();
        }
        
        private void calcSymvolCount()
        {
            label1.Text = "Символов: " + richTextBox1.Text.Trim().Replace(" ", "").Length.ToString();
            probelLbl.Text = "Символов с пробелами: " + richTextBox1.Text.Length.ToString();
        }
        
        private string[] GetWords(string text)
        {
            if (text.Length == 0)
            {
                return new string[0];
            }

            text = text
                .Replace("?", "^")
                .Replace("!", "^")
                .Replace(":", "^")
                .Replace("(", "^")
                .Replace("#", "^")
                .Replace("№", "^")
                .Replace(")", "^")
                .Replace(" ", "^")
                .Replace(",", "^")
                .Replace(" - ", "^")
                .Replace("—", "^")
                .Replace("–", "^")                
                .Replace("-", "^")
                .Replace(".", "^")
                .Replace(";", "^")
                .Replace("\t", "^")
                .Replace("\n", "^")
                .Replace("\r\n", "^")
                .ToLower();

            return text.Split(new Char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        public bool ShouldRemove(string s)
        {
            return s.Length < 4;
        }

        public HashSet<string> GetWordsToHighlight()
        {
            HashSet<string> text = new HashSet<string>(GetWords(richTextBox1.Text));
            HashSet<string> combined1 = new HashSet<string>(GetWords(richTextBox2.Text));
            HashSet<string> combined2 = new HashSet<string>(GetWords(richTextBox3.Text));
            HashSet<string> combined3 = new HashSet<string>(GetWords(richTextBox4.Text));
            HashSet<string> combined4 = new HashSet<string>(GetWords(richTextBox5.Text));
            HashSet<string> combined5 = new HashSet<string>(GetWords(richTextBox6.Text));
            HashSet<string> combined6 = new HashSet<string>(GetWords(richTextBox7.Text));
            HashSet<string> combined7 = new HashSet<string>(GetWords(richTextBox8.Text));

            combined1.UnionWith(combined2);
            combined1.UnionWith(combined3);
            combined1.UnionWith(combined4);
            combined1.UnionWith(combined5);
            combined1.UnionWith(combined6);
            combined1.UnionWith(combined7);

            combined2 = null;
            combined3 = null;
            combined4 = null;
            combined5 = null;
            combined6 = null;
            combined7 = null;

            if (combined1.Contains(null))
            {
                combined1.Remove(null);
            }

            combined1.RemoveWhere(ShouldRemove);

            return combined1;
        }

        private void btnPlan_Click(object sender, EventArgs e)
        {
            ShowPlanForm();
        }

        public void ShowPlanForm()
        {
            PlanForm planForm = new PlanForm();
            planForm.SetPlan(plan);
            DialogResult res = planForm.ShowDialog();

            if (res == DialogResult.Yes)
            {
                plan = planForm.GetPlan();

                AddToHistory();
            }
        }

        public void ShowKeysForm()
        {
            KeysForm keysForm = new KeysForm();
            keysForm.SetKeys(this.words);

            DialogResult res = keysForm.ShowDialog();

            if (res == DialogResult.Yes)
            {
                words = keysForm.GetKeyStringsArray();
                SortKeywords();
                AddToHistory();
            }
        }
            
        private void btnKeys_Click(object sender, EventArgs e)
        {
            ShowKeysForm();
        }

        private void планToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPlanForm();
        }

        private void ключевыеСловаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowKeysForm();
        }

        private void копироватьТекстToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyText();
        }
                
        private void синонимыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = richTextBox1.SelectedText.Trim();

            if ("" != text || text.IndexOf(" ") != -1)
            {
                
                text = HttpUtility.UrlEncode(text);

                Process.Start("http://text.ru/synonym/" + text);

                //Process.Start("http://словарь-синонимов.рф/words?keyword=" + text);
            }
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory();

            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            richTextBox4.Text = "";
            richTextBox5.Text = "";
            richTextBox6.Text = "";
            richTextBox7.Text = "";
            richTextBox8.Text = "";
            plan = "";
            words.Clear();
            history.Clear();
            currentFile = "";
            tempWords.Clear();
            tempRichText.Text = "";
            this.Text = "Рерайт";
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory();

            if ("" == currentFile)
            {
                SaveAs();
            }
            else
            {
                Save(currentFile, true, null);
            }
        }

        public bool IsEmptyDocument()
        {
            if (richTextBox1.Text == ""
                && richTextBox2.Text == ""
                && richTextBox3.Text == ""
                && richTextBox4.Text == ""
                && richTextBox5.Text == ""
                && richTextBox6.Text == ""
                && richTextBox7.Text == ""
                && richTextBox8.Text == ""
                && plan == ""
                && words.Count == 0)
            {
                return true;
            }

            return false;
        }

        public XmlDocument GetCurDocument()
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                XmlNode declaration = doc.CreateNode(XmlNodeType.XmlDeclaration, "declaration", null);
                doc.AppendChild(declaration);

                XmlNode mainNode = doc.CreateElement("rewrite");
                doc.AppendChild(mainNode);

                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox1", richTextBox1.Text));
                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox2", richTextBox2.Text));
                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox3", richTextBox3.Text));
                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox4", richTextBox4.Text));
                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox5", richTextBox5.Text));
                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox6", richTextBox6.Text));
                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox7", richTextBox7.Text));
                mainNode.AppendChild(GetXmlTextNode(doc, "richTextBox8", richTextBox8.Text));

                mainNode.AppendChild(GetXmlTextNode(doc, "plan", plan));

                XmlNode wordsNode = doc.CreateElement("words");
                mainNode.AppendChild(wordsNode);

                for (int i = 0; i < words.Count; i++)
                {
                    XmlNode wordNode = doc.CreateElement("word");

                    XmlAttribute atr = doc.CreateAttribute("xx", "count", "somens");
                    atr.Value = words[i].count.ToString();
                    wordNode.Attributes.Append(atr);

                    XmlAttribute atr2 = doc.CreateAttribute("xx", "doneCount", "somens");
                    atr2.Value = words[i].doneCount.ToString();
                    wordNode.Attributes.Append(atr2);

                    wordNode.InnerText = words[i].text;
                    wordsNode.AppendChild(wordNode);
                }

                XmlNode historyNode = doc.CreateElement("history");
                mainNode.AppendChild(historyNode);

                for (int i = 0; i < history.Count; i++)
                {
                    XmlNode histNode = doc.CreateElement("hist");
                    XmlAttribute atr = doc.CreateAttribute("xx", "title", "somens");
                    atr.Value = history[i].Title;
                    histNode.Attributes.Append(atr);
                    histNode.InnerText = history[i].Text;
                    historyNode.AppendChild(histNode);
                }

                return doc;
            }
            catch
            {
                return null;
            }
        }

        public void Save(string filename, bool isMainSave, XmlDocument customDocument)
        {
            try
            {
                XmlDocument doc = (customDocument != null) ? customDocument : GetCurDocument();
                
                if (File.Exists(filename) && !isMainSave)
                {
                    File.SetAttributes(filename, FileAttributes.Normal);
                    File.Delete(filename);
                }

                doc.Save(filename);

                if (isMainSave)
                {
                    currentFile = filename;
                    this.Text = "Рерайт: \"" + filename + "\"";
                }
                else
                {                    
                    File.SetAttributes(filename, FileAttributes.Hidden);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения");
            }
        }

        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "rewrite files (*.rwt)|*.rwt";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Save(sfd.FileName, true, null);
            }
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        
        public XmlNode GetXmlTextNode(XmlDocument doc, string name, string text)
        {
            XmlNode mainText = doc.CreateElement(name);
            mainText.InnerText = text;

            return mainText;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory();

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "rewrite files (*.rwt)|*.rwt";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenDocument(ofd.FileName, false);
            }
        }
        
        private void уникальностьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory();

            Process.Start("http://text.ru/antiplagiat");
        }

        private void сЕОАнализToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory(); 
            
            Process.Start("http://advego.ru/text/seo/top/");
        }

        private void орфограмкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory(); 
            
            Process.Start("http://cabinet.orfogrammka.ru/");
        }

        private void синонимыToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddToHistory();

            Process.Start("http://www.classes.ru/all-russian/russian-dictionary-synonyms.htm");
        }

        private void btnCopy_Click_1(object sender, EventArgs e)
        {
            RichTextBox curText = null;

            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    curText = richTextBox2;
                    break;
                case "tabPage2":
                    curText = richTextBox3;
                    break;
                case "tabPage3":
                    curText = richTextBox4;
                    break;
                case "tabPage4":
                    curText = richTextBox5;
                    break;
                case "tabPage5":
                    curText = richTextBox6;
                    break;
                case "tabPage6":
                    curText = richTextBox7;
                    break;
                case "tabPage7":
                    curText = richTextBox8;
                    break;

                default:
                    break;
            }

            if (curText != null)
            {
                curText.Copy();
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            AddToHistory();

            richTextBox1.Focus();
            richTextBox1.Paste();
        }

        private void copyAllText_Click(object sender, EventArgs e)
        {
            CopyText();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsEmptyDocument())
            {
                OnExitForm exitForm = new OnExitForm();
                DialogResult res = exitForm.ShowDialog();

                if (res == DialogResult.Yes)
                {
                    if ("" == currentFile)
                    {
                        SaveAs();
                    }
                    else
                    {
                        Save(currentFile, true, null);
                    }

                    Save(autosaveFilename, false, null);
                }
                else if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (!IsEmptyDocument())
                    {
                        Save(autosaveFilename, false, null);
                    }
                }
            }
        }

        private void просмотрИсторииИзмененийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (history.Count > 0)
            {
                HistoryForm f = new HistoryForm();

                f.LoadHistory(history);

                if (f.ShowDialog() == DialogResult.Yes)
                {
                    AddToHistory();
                    richTextBox1.Text = f.historyRichTextBox.Text;
                }
            }
            else
            {
                MessageBox.Show("Нет истории", "Ошибка");
            }
        }

        private void копироватьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "")
            {
                richTextBox1.Copy();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (richTextBox1.SelectedText != "")
            {
                contextMenuStrip1.Items[0].Enabled = true;
                contextMenuStrip1.Items[1].Enabled = true;
                contextMenuStrip1.Items[3].Enabled = true;
                contextMenuStrip1.Items[4].Enabled = true;
                contextMenuStrip1.Items[5].Enabled = true;
            }
            else
            {
                contextMenuStrip1.Items[0].Enabled = false;
                contextMenuStrip1.Items[1].Enabled = false;
                contextMenuStrip1.Items[3].Enabled = false;
                contextMenuStrip1.Items[4].Enabled = false;
                contextMenuStrip1.Items[5].Enabled = false;
            }
        }
        
        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Focus();
            richTextBox1.Paste();
        }

        private void жирныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "")
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
            }
        }

        private void курсивToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "")
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Italic);
            }
        }
        
        private void синонимыToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string text = richTextBox1.SelectedText.Trim().ToLower();

            if (text != "")
            {
                MessageBox.Show(synonims[text], "Синонимы к слову: " + text);
                
            }
        }

        private void обычныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "")
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
            }
        }

        private void OpenDocument(string filename, bool isLastDocument)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNode mainNode = doc.SelectSingleNode("rewrite");

                richTextBox1.Text = mainNode.SelectSingleNode("richTextBox1").InnerText;
                richTextBox2.Text = mainNode.SelectSingleNode("richTextBox2").InnerText;
                richTextBox3.Text = mainNode.SelectSingleNode("richTextBox3").InnerText;
                richTextBox4.Text = mainNode.SelectSingleNode("richTextBox4").InnerText;
                richTextBox5.Text = mainNode.SelectSingleNode("richTextBox5").InnerText;
                richTextBox6.Text = mainNode.SelectSingleNode("richTextBox6").InnerText;
                richTextBox7.Text = mainNode.SelectSingleNode("richTextBox7").InnerText;
                richTextBox8.Text = mainNode.SelectSingleNode("richTextBox8").InnerText;
                plan = mainNode.SelectSingleNode("plan").InnerText;

                XmlNode wordsNode = mainNode.SelectSingleNode("words");
                words.Clear();
                foreach (XmlNode w in wordsNode)
                {
                    Key key = new Key();

                    if (w.Attributes.Count > 2) 
                    {
                        key.count = int.Parse(w.Attributes[0].Value);
                        key.doneCount = int.Parse(w.Attributes[1].Value);
                    }
                    else 
                    {
                        key.count = 1;
                        key.doneCount = 0;
                    }

                    
                    key.text = w.InnerText;
                    words.Add(key);
                }

                XmlNode historyNode = mainNode.SelectSingleNode("history");
                history.Clear();

                if (historyNode != null)
                {
                    foreach (XmlNode hist in historyNode)
                    {
                        History h = new History();
                        h.Title = hist.Attributes[0].Value;
                        h.Text = hist.InnerText;
                        history.Add(h);
                    }   
                }

                if (isLastDocument)
                {
                    currentFile = "";
                    this.Text = "Рерайт: Последний документ";
                }
                else
                {
                    currentFile = filename;
                    this.Text = "Рерайт: \"" + currentFile + "\"";
                }
                SortKeywords();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private void openLastDocumentMenu_Click(object sender, EventArgs e)
        {
            AddToHistory();

            if (File.Exists(autosaveFilename))
            {
                OpenDocument(autosaveFilename, true);
            }
            else
            {
                MessageBox.Show("Нет сохраненных предыдущих документов", "Ошибка");
            }
        }

        public void AddToHistory()
        {
            if (richTextBox1.Text.Trim() != "")
            {
                if (history.Count == 0 || history.Count > 0 && richTextBox1.Text.Trim() != history[0].Text.Trim())
                {
                    History h = new History();
                    h.Title = System.DateTime.Now.ToString();
                    h.Text = richTextBox1.Text.Trim();

                    history.Insert(0, h);

                    if (history.Count > 50)
                    {
                        history.RemoveAt(50);
                    }
                }
            }
        }

        private void clearSearchBtn_Click(object sender, EventArgs e)
        {
            searchTxt.Text = "";
            searchLabel.Text = "";
        }

        private void searchTxt_TextChanged(object sender, EventArgs e)
        {
            if (searchTxt.Text.Trim() != "") 
            {
                searchLabel.Show();
                clearSearchBtn.Show();
            } 
            else 
            {
                searchLabel.Hide();
                clearSearchBtn.Hide();
            }

            //HighlightKeywords(richTextBox1);
        }

        private void unusedTxt_Enter(object sender, EventArgs e)
        {
            richTextBox1.Focus();
        }

        private void синонимыToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tabControl1.SelectedTab.Controls.Count; i++)
            {
                RichTextBox r = (RichTextBox)tabControl1.SelectedTab.Controls[i];
                string text = r.SelectedText.Trim().ToLower();

                if (text != "")
                {
                    MessageBox.Show(synonims[text], "Синонимы к слову: " + text);

                }
                break;
            }
        }

        public void SortKeywords()
        {
            tempWords = null;
            tempWords = new List<Key>();
            for (int i = 0; i < this.words.Count; i++)
            {
                this.words[i].doneCount = 0;
                tempWords.Add(new Key() { text = this.words[i].text, count = this.words[i].count, doneCount = this.words[i].doneCount });
            }

            if (tempWords.Count > 0)
            {
                for (int i = 0; i < tempWords.Count - 1; i++)
                {
                    for (int j = 0; j < tempWords.Count - i - 1; j++)
                    {
                        if (tempWords[j].text.Length < tempWords[j + 1].text.Length)
                        {
                            Key tmp = tempWords[j];

                            tempWords[j] = tempWords[j + 1];
                            tempWords[j + 1] = tmp;

                            tmp = null;
                        }
                    }
                }
            }
        }

        private void saveAsWordMenu_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Word files (*.rtf)|*.rtf";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                RichTextBox tmp = new RichTextBox();
                tmp.Rtf = richTextBox1.Rtf;
                tmp.SelectionStart = 0;
                tmp.SelectionLength = tmp.Text.Length;
                tmp.SelectionBackColor = Color.White;
                tmp.SaveFile(sfd.FileName, RichTextBoxStreamType.RichText);
            }
        }

        private void pasteContextMenuStringTabs_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tabControl1.SelectedTab.Controls.Count; i++)
            {
                RichTextBox r = (RichTextBox)tabControl1.SelectedTab.Controls[i];
                
                r.Focus();
                r.Paste();
                break;
            }
        }

        private void contextMenuStringAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Версия: 0.12\r\n\r\nСоздатель: mepata@yandex.ru", "О программе");
        }

        private void contMenuStringActivation_Click(object sender, EventArgs e)
        {
            Activation frm = new Activation();
            frm.SetValues(curProgramCode, curProgramRunCount);

            if (frm.ShowDialog() == DialogResult.Yes)
            {
                doActivation(frm.txtActivationCode.Text.Trim());
            }
        }

        private void поддержитеПроектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Donate frm = new Donate();
            frm.ShowDialog();
        }

        public void doActivation(string code)
        {            
            if (code == (curProgramCode + "Im Pata: ").GetHashCode().ToString())
            {
                AddToRegistry("RewriteProgramRunCounter", "20000");
                curProgramRunCount = 20000;

                this.Text = "Рерайт";
                panel2.Enabled = true;
                menuStrip1.Enabled = true;
                файлToolStripMenuItem.Enabled = true;
                копироватьToolStripMenuItem.Enabled = true;
                сайтыToolStripMenuItem.Enabled = true;
                планToolStripMenuItem.Enabled = true;
                ключевыеСловаToolStripMenuItem.Enabled = true;
                contextMenuAboutProgramMain.Enabled = true;
                таймерToolStripMenuItem.Enabled = true;

                MessageBox.Show("Программа успешно активирована", "Активация");
            }
            else
            {
                MessageBox.Show("Не верный код активации", "Активация");
            }
        }

        private void отключитьПодсветкуСовпаденийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dontShowMatchedWords = отключитьПодсветкуСовпаденийToolStripMenuItem.Checked;

            AddToRegistry("RewriteProgramShoMatchedWords", this.dontShowMatchedWords.ToString());
                
        }

        private void отключитьПодсветкуКлючейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dontShowMatchedKeysWords = отключитьПодсветкуКлючейToolStripMenuItem.Checked;

            AddToRegistry("RewriteProgramShoMatchedKeysWords", this.dontShowMatchedKeysWords.ToString());
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            RichTextBox curText = null;

            switch (tabControl1.SelectedTab.Name)
            {
                case "tabPage1":
                    curText = richTextBox2;
                    break;
                case "tabPage2":
                    curText = richTextBox3;
                    break;
                case "tabPage3":
                    curText = richTextBox4;
                    break;
                case "tabPage4":
                    curText = richTextBox5;
                    break;
                case "tabPage5":
                    curText = richTextBox6;
                    break;
                case "tabPage6":
                    curText = richTextBox7;
                    break;
                case "tabPage7":
                    curText = richTextBox8;
                    break;

                default:
                    break;
            }

            if (curText != null)
            {
                LockWindow(this.Handle);
                int scrollPos = GetScrollPos(curText.Handle, 1);
                bool isFocused = curText.Focused;

                int startSel = curText.SelectionStart;
                int startLen = curText.SelectionLength;
                curText.HideSelection = true;

                curText.SelectAll();
                curText.SelectionBackColor = Color.White;
                curText.DeselectAll();

                curText.SelectionStart = startSel;
                curText.SelectionLength = startLen;
                curText.SelectionBackColor = Color.White;
                ScrollTo(scrollPos);
                LockWindow(IntPtr.Zero);
            }
        }

        private void главредToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AddToHistory();

            Process.Start("https://glvrd.ru/");
        }

        private void стартТаймераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory();

            workTimeTimer.Start();
            стартТаймераToolStripMenuItem.Enabled = false;
            стопТаймераToolStripMenuItem.Enabled = true;
            обнулитьВремяToolStripMenuItem.Enabled = false;
        }

        private void стопТаймераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToHistory();

            workTimeTimer.Stop();
            стартТаймераToolStripMenuItem.Enabled = true;
            стопТаймераToolStripMenuItem.Enabled = false;
            обнулитьВремяToolStripMenuItem.Enabled = true;
        }

        private void обнулитьВремяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            labelWorkTime.Text = "00:00";
            стартТаймераToolStripMenuItem.Enabled = true;
            стопТаймераToolStripMenuItem.Enabled = false;
        }

        private void заменитьНаToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            string text = richTextBox1.SelectedText.Trim().ToLower();
            заменитьНаToolStripMenuItem.DropDownItems.Clear();

            if (text != "")
            {
                string syn = synonims[text];

                if (syn != null && syn != "")
                {
                    string[] res = syn.Split(',');

                    foreach (string word in res)
                    {
                        заменитьНаToolStripMenuItem.DropDownItems.Add(word, null, пустоToolStripMenuItem_Click);
                    }
                }
                else
                {
                    заменитьНаToolStripMenuItem.DropDownItems.Add("(нет синонимов)");
                }
            }
            else
            {
                заменитьНаToolStripMenuItem.DropDownItems.Add("(нет синонимов)");
            }
        }

        private void пустоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;

            string word = t.Text;
            string text = richTextBox1.SelectedText;

            text = text.Replace(text.Trim(), word);

            richTextBox1.SelectedText = text;
        }

        private void отключитьПодсветкуОшибокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dontShowWrongWords = отключитьПодсветкуОшибокToolStripMenuItem.Checked;

            AddToRegistry("RewriteProgramShoWrongWords", this.dontShowWrongWords.ToString());
        }
    }


    public struct FoundWords
    {
        public string text;
        public int start;
        public int length;
    }

}