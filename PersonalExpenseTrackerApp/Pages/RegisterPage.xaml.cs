using PersonalExpenseTrackerUI.Services;

namespace PersonalExpenseTrackerUI.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;

    public RegisterPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        var newUser = new
        {
            username = usernameEntry.Text,
            email = emailEntry.Text,
            passwordHash = passwordEntry.Text,
            firstName = firstNameEntry.Text,
            lastName = lastNameEntry.Text,
            phoneNumber = phoneNumberEntry.Text,
            dateOfBirth = dobEntry.Text
        };

        var response = await _apiService.RegisterUserAsync(newUser);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("Success", "User registered successfully!", "OK");
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Error", $"Registration failed: {errorMessage}", "OK");
        }
    }
}
