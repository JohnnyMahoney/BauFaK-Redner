using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BauFakCounter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CurrentNumberView.Content = null;
        }

        List<SpeakerNumber> UpcomingList = new List<SpeakerNumber>();
        List<SpeakerNumber> RednerLog = new List<SpeakerNumber>();
        SpeakerNumber CurrentNumber = new SpeakerNumber();
        int RednerPosition = 1;



        private void UpcomingNumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void UpcomingNumberInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var item = new SpeakerNumber();
                item.RednerNummer = UpcomingNumberInput.Text;
                UpcomingList.Add(item);
                UpcomingNumberView.Items.Add(item.RednerNummer);
                UpcomingNumberInput.Text = "";
            }
        }

        private void ButtonNextNumber_Click(object sender, RoutedEventArgs e)
        {
            if (UpcomingList.Count == 0)
            {
                if (CurrentNumber.RednerNummer != null)
                {
                    CurrentToLog();
                }
                MessageBox.Show("Das Ende der Rednerliste ist erreicht. Bitte Rednernummern hinzufügen.");

            }
            else
            {
                CurrentToLog();
                UpcomingToCurrent();

            }


        }

        private void UpcomingNumberView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpcomingList.RemoveAt(UpcomingNumberView.SelectedIndex);
            UpcomingNumberView.Items.RemoveAt(UpcomingNumberView.SelectedIndex);

        }

        private void ButtonSaveList_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.Filter = ".csv Datei (*.csv)|*.csv";
            saveFileDialog.FilterIndex = 1;
            if (saveFileDialog.ShowDialog() == true && RednerLog.Count != 0)
            {
                try
                {
                    File.AppendAllText(saveFileDialog.FileName, "Rednernummer;Redner;Redebeginn;Redeende" + Environment.NewLine);
                    for (int i = 0; i < RednerLog.Count; i++)
                    {
                        File.AppendAllText(saveFileDialog.FileName, RednerLog[i].RednerReihenfolge + ";" + RednerLog[i].RednerNummer + ";" + RednerLog[i].RedeAnfang + ";" + RednerLog[i].RedeEnde + Environment.NewLine);

                    }
                    if (CurrentNumber.RednerNummer != null)
                    {
                        CurrentNumber.RedeEnde = DateTime.Now;
                        CurrentNumber.RednerReihenfolge = RednerPosition++;
                        File.AppendAllText(saveFileDialog.FileName, CurrentNumber.RednerReihenfolge + ";" + CurrentNumber.RednerNummer + ";" + CurrentNumber.RedeAnfang + ";" + CurrentNumber.RedeEnde + Environment.NewLine);
                    }
                    MessageBox.Show("Datei wurde gespeichert.");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler: Konnte Datei nicht speichern. " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Fehler: Bitte noch einmal versuchen!");

            }
        }

        private void CurrentToLog()
        {
            var temp = new SpeakerNumber();
            if (CurrentNumber.RednerNummer != null)
            {
                temp.RednerReihenfolge = RednerPosition++;
                temp.RednerNummer = CurrentNumber.RednerNummer;
                temp.RedeAnfang = CurrentNumber.RedeAnfang;
                temp.RedeEnde = DateTime.Now;
                RednerLog.Add(temp);
            }
            CurrentNumberView.Content = null;
            CurrentNumber.RednerNummer = null;
        }
        private void UpcomingToCurrent()
        {
            CurrentNumber.RednerNummer = UpcomingList[0].RednerNummer;
            CurrentNumber.RedeAnfang = DateTime.Now;
            CurrentNumberView.Content = CurrentNumber.RednerNummer;
            UpcomingList.RemoveAt(0);
            UpcomingNumberView.Items.RemoveAt(0);


        }
    }
}
