using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csv
{
    /// <summary>
    /// </summary>
    public partial class ExpanseItHome : Page
    {
        public ExpanseItHome()
        {
            InitializeComponent();
        }

        private void viewButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Pick a CSV.";

            if(dialog.ShowDialog() == true)
            {
                if (radioNormarize.IsChecked.GetValueOrDefault(false))
                {
                    Normalize(dialog.FileName);
                }
                else if(radioMean.IsChecked.GetValueOrDefault(false))
                {
                    var column = 1;
                    extractColumn(dialog.FileName, column, new Action<List<int>>(list => {
                        Log($"The mean of column {column} is {computeMean(list)}");
                    }));
                }
                else if (radioSD.IsChecked.GetValueOrDefault(false))
                {
                    var column = 1;
                    extractColumn(dialog.FileName, column, new Action<List<int>>(list => {
                        Log($"The standard deviation of column {column} is {computeSd(computeMean(list), list)}");
                    }));
                }
                else
                {
                    Log("proguramu kowareteru");
                }

            }
            else
            {
                Log("No file was chosen.");
            }
        }

        private async void Normalize(string FileName)
        {
            await Task.Run(() =>
            {
                using (var writer = TextWriter.Synchronized(File.CreateText("normalized.csv")))
                {
                    var start = DateTime.Now;
                    Log($"Started processing {FileName}.");
                    var file = File.ReadLines(FileName);
                    initializeIndicator(file.Count());
                    writer.WriteLine(file.First());
                    Parallel.ForEach(file.Skip(1), it => {
                        var line = new List<string>(/* capacity = */3);
                        foreach(var v in it.Split(',')) { /* CSVの仕様上カンマ区切りだけとみなすと壊れるかもしれないがとりあえず大丈夫そうなのでこれで行く */
                            line.Add(stripNonNumericFrom(v));
                        }
                        writer.WriteLine(string.Join(",", line)); // 書き込みが別スレッドという要件にマッチしているかよくわからないので調べる
                        refreshIndicator(1, false);
                    });
                    refreshIndicator(0, true);
                    var end = DateTime.Now;
                    Log($"Finished processing. {(end - start).TotalSeconds.ToString()} secs has elapsed.");
                }
            });
        }

        private double computeMean(List<int> values) {
            double sum = 0.0;
            foreach(var i in values)
            {
                sum += i;
            }
            return sum / values.Count();
        }

        private double computeSd(double mean, List<int> values)
        {
            var variance = 0.0;
            foreach(var v in values)
            {
                variance += Math.Pow((mean - v), 2.0) / values.Count();
            }
            return Math.Sqrt(variance);
        }

        private async void extractColumn(string fileName, int columnIndex, Action<List<int>> afterExtracted)
        {
            var lockObject = new Object();
            await Task.Run(() =>
            {
                var file = File.ReadLines(fileName);
                var result = new List<int>(file.Count());
                Parallel.ForEach(file, it => {
                    var value = 0;
                    var str = it.Split(',')[columnIndex];
                    var success = int.TryParse(str, out value);
                    if (success)
                    {
                        result.Add(value);
                    }
                    else
                    {
                        Log($"Failing conversion string(value: {str} to int.)");
                    }
                });
                afterExtracted.Invoke(result);
            });
        }

        private int counter = 0;

        private void refreshIndicator(int increment, bool forceRefresh)
        {
            counter += increment;
            if(counter % 500 == 0 || forceRefresh) {
                progressBar1.Dispatcher.Invoke(new Action(() => {
                    progressBar1.Value = counter;
                    labelCountNow.Content = counter.ToString();
                }));
            }
        }

        private void initializeIndicator(int maximum)
        {
            counter = 0;
            progressBar1.Dispatcher.Invoke(new Action(() => {
                    progressBar1.Value = 0;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = maximum;
                    labelCountEnd.Content = maximum;
            }));
        }

        private string stripNonNumericFrom(string str)
        {
            return convertToHankakuFrom(System.Text.RegularExpressions.Regex.Replace(str, @"[^0-9０-９]+", ""));
        }

        private string convertToHankakuFrom(string zenkakuString)
        {
            var b = new StringBuilder("", zenkakuString.Length);
            foreach(var c in zenkakuString.ToCharArray())
            {
                switch(c)
                {
                    case '０': b.Append('0'); break;
                    case '１': b.Append('1'); break;
                    case '２': b.Append('2'); break;
                    case '３': b.Append('3'); break;
                    case '４': b.Append('4'); break;
                    case '５': b.Append('5'); break;
                    case '６': b.Append('6'); break;
                    case '７': b.Append('7'); break;
                    case '８': b.Append('8'); break;
                    case '９': b.Append('9'); break;
                    default: b.Append(c); break;
                        
                }
            }
            return b.ToString();
        }

        private void Log(string content)
        {
            var message = DateTime.Now.ToString() + " " + content;
            textBoxLog.Dispatcher.Invoke(new Action(() =>
            {
                textBoxLog.Text += $"{DateTime.Now} {content}{System.Environment.NewLine}";
            }));
        }
    }
}
