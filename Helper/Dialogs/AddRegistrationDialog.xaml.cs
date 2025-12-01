using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedicalAir.Model.Entites;

namespace MedicalAir.Helper.Dialogs
{
    public partial class AddRegistrationDialog : Window
    {
        private readonly IEnumerable<User> _users;
        private readonly IEnumerable<Airplane> _airplanes;

        public int SelectedUserId { get; set; }
        public int SelectedAirplaneId { get; set; }
        public string MessageBody { get; set; }
        public DateTime SelectedDate { get; set; }

        public AddRegistrationDialog(IEnumerable<User> users, IEnumerable<Airplane> airplanes)
        {
            InitializeComponent();
            
            _users = users;
            _airplanes = airplanes;
            
            UserComboBox.ItemsSource = users;
            UserComboBox.SelectionChanged += UserComboBox_SelectionChanged;
            
            DatePicker.SelectedDate = DateTime.Now;
            SelectedDate = DateTime.Now;
        }

        public void SetValues(int userId, int airplaneId, string messageBody, DateTime date)
        {
            SelectedUserId = userId;
            SelectedAirplaneId = airplaneId;
            MessageBody = messageBody ?? "";
            SelectedDate = date;

            // Устанавливаем значения в UI
            UserComboBox.SelectedValue = userId;
            DatePicker.SelectedDate = date;
            MessageTextBox.Text = MessageBody;

            // Обновляем самолеты для выбранного пользователя
            var selectedUser = _users.FirstOrDefault(u => u.Id == userId);
            if (selectedUser != null)
            {
                // Фильтруем самолеты - показываем только самолет, закрепленный за пользователем
                if (selectedUser.AirplaneId.HasValue)
                {
                    var userAirplane = _airplanes.FirstOrDefault(a => a.Id == selectedUser.AirplaneId.Value);
                    if (userAirplane != null)
                    {
                        AirplaneComboBox.ItemsSource = new List<Airplane> { userAirplane };
                        AirplaneComboBox.SelectedValue = airplaneId;
                    }
                }
            }
        }

        public void SetEditMode()
        {
            // Изменяем заголовок и текст кнопки для режима редактирования
            var titleTextBlock = this.FindName("TitleTextBlock") as System.Windows.Controls.TextBlock;
            if (titleTextBlock != null)
            {
                titleTextBlock.Text = "Редактирование регистрации";
            }

            var okButton = this.FindName("OkButton") as System.Windows.Controls.Button;
            if (okButton != null)
            {
                okButton.Content = "Сохранить";
            }
        }

        private void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserComboBox.SelectedItem is User selectedUser)
            {
                // Фильтруем самолеты - показываем только самолет, закрепленный за пользователем
                if (selectedUser.AirplaneId.HasValue)
                {
                    var userAirplane = _airplanes.FirstOrDefault(a => a.Id == selectedUser.AirplaneId.Value);
                    if (userAirplane != null)
                    {
                        AirplaneComboBox.ItemsSource = new List<Airplane> { userAirplane };
                        AirplaneComboBox.SelectedValue = userAirplane.Id;
                    }
                    else
                    {
                        AirplaneComboBox.ItemsSource = new List<Airplane>();
                        ModernMessageDialog.Show("У выбранного пользователя не назначен самолет!", "Предупреждение", MessageType.Warning);
                    }
                }
                else
                {
                    AirplaneComboBox.ItemsSource = new List<Airplane>();
                    ModernMessageDialog.Show("У выбранного пользователя не назначен самолет!", "Предупреждение", MessageType.Warning);
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedValue == null)
            {
                ModernMessageDialog.Show("Выберите пользователя!", "Ошибка", MessageType.Warning);
                return;
            }

            if (AirplaneComboBox.SelectedValue == null)
            {
                ModernMessageDialog.Show("Выберите самолет!", "Ошибка", MessageType.Warning);
                return;
            }

            if (DatePicker.SelectedDate == null)
            {
                ModernMessageDialog.Show("Выберите дату!", "Ошибка", MessageType.Warning);
                return;
            }

            SelectedUserId = (int)UserComboBox.SelectedValue;
            SelectedAirplaneId = (int)AirplaneComboBox.SelectedValue;
            MessageBody = MessageTextBox.Text ?? "";
            SelectedDate = DatePicker.SelectedDate.Value;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

