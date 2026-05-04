using System.Windows;
using RoomReservation.Desktop.Services;
using RoomReservation.Domain.Models;

namespace RoomReservation.Desktop
{
	public partial class MainWindow : Window
	{
		private readonly ApiClient _apiClient = new ApiClient();

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void LoadRooms_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				RoomsGrid.ItemsSource = await _apiClient.GetRoomsAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot load rooms: " + ex.Message);
			}
		}

		private async void AddRoom_Click(object sender, RoutedEventArgs e)
		{
			var window = new AddRoomWindow();

			if (window.ShowDialog() == true && window.Room != null)
			{
				try
				{
					await _apiClient.CreateRoomAsync(window.Room);
					RoomsGrid.ItemsSource = await _apiClient.GetRoomsAsync();
					MessageBox.Show("Room created.");
				}
				catch (Exception ex)
				{
					MessageBox.Show("Cannot create room: " + ex.Message);
				}
			}
		}

		private async void DeleteRoom_Click(object sender, RoutedEventArgs e)
		{
			if (RoomsGrid.SelectedItem is not Room room)
			{
				MessageBox.Show("Select room first.");
				return;
			}

			var result = MessageBox.Show(
				"Do you really want to delete selected room?",
				"Confirm delete",
				MessageBoxButton.YesNo);

			if (result != MessageBoxResult.Yes)
			{
				return;
			}

			try
			{
				await _apiClient.DeleteRoomAsync(room.Id);
				RoomsGrid.ItemsSource = await _apiClient.GetRoomsAsync();
				MessageBox.Show("Room deleted.");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot delete room: " + ex.Message);
			}
		}

		private async void LoadReservations_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ReservationsGrid.ItemsSource = await _apiClient.GetReservationsAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot load reservations: " + ex.Message);
			}
		}

		private async void LoadUsers_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				UsersGrid.ItemsSource = await _apiClient.GetUsersAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot load users: " + ex.Message);
			}
		}

		private async void DeleteUser_Click(object sender, RoutedEventArgs e)
		{
			if (UsersGrid.SelectedItem is not User user)
			{
				MessageBox.Show("Select user first.");
				return;
			}

			var result = MessageBox.Show(
				"Do you really want to delete selected user?",
				"Confirm delete",
				MessageBoxButton.YesNo);

			if (result != MessageBoxResult.Yes)
			{
				return;
			}

			try
			{
				await _apiClient.DeleteUserAsync(user.Id);
				UsersGrid.ItemsSource = await _apiClient.GetUsersAsync();
				MessageBox.Show("User deleted.");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot delete user: " + ex.Message);
			}
		}


		private async void CancelReservation_Click(object sender, RoutedEventArgs e)
		{
			if (ReservationsGrid.SelectedItem is not Reservation reservation)
			{
				MessageBox.Show("Select reservation first.");
				return;
			}

			try
			{
				await _apiClient.CancelReservationAsync(reservation.Id);
				ReservationsGrid.ItemsSource = await _apiClient.GetReservationsAsync();
				MessageBox.Show("Reservation cancelled.");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Cannot cancel reservation: " + ex.Message);
			}
		}
	}
}