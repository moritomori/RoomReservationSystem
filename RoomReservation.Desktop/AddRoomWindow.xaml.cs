using System.Windows;
using RoomReservation.Domain.Models;

namespace RoomReservation.Desktop
{
	public partial class AddRoomWindow : Window
	{
		public Room? Room { get; private set; }

		public AddRoomWindow()
		{
			InitializeComponent();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(NameTextBox.Text))
			{
				MessageBox.Show("Room name is required.");
				return;
			}

			if (!int.TryParse(CapacityTextBox.Text, out int capacity) || capacity <= 0)
			{
				MessageBox.Show("Capacity must be greater than zero.");
				return;
			}

			if (!int.TryParse(MaxDurationTextBox.Text, out int maxDuration) || maxDuration <= 0)
			{
				MessageBox.Show("Max duration must be greater than zero.");
				return;
			}

			Room = new Room
			{
				Name = NameTextBox.Text,
				Capacity = capacity,
				Equipment = EquipmentTextBox.Text,
				MaxReservationDurationMinutes = maxDuration
			};

			DialogResult = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}