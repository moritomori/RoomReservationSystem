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