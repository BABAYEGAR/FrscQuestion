namespace FrscQuestion.Models.Encryption
{
    public class Hashing
    {
        private string GetRandomSalt()
        {
            return BCrypt.GenerateSalt(12);
        }

        public string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, GetRandomSalt());
        }

        public bool ValidatePassword(string password, string correctHash)
        {
            return BCrypt.Verify(password, correctHash);
        }
    }
}