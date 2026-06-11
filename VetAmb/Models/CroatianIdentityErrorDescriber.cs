using Microsoft.AspNetCore.Identity;

namespace VetAmb.Models
{
    public class CroatianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
            => new() { Code = nameof(DefaultError), Description = "Došlo je do nepoznate pogreške." };

        public override IdentityError ConcurrencyFailure()
            => new() { Code = nameof(ConcurrencyFailure), Description = "Optimistično zaključavanje nije uspjelo, objekt je izmijenjen." };

        public override IdentityError PasswordMismatch()
            => new() { Code = nameof(PasswordMismatch), Description = "Neispravna lozinka." };

        public override IdentityError InvalidToken()
            => new() { Code = nameof(InvalidToken), Description = "Nevažeći token." };

        public override IdentityError LoginAlreadyAssociated()
            => new() { Code = nameof(LoginAlreadyAssociated), Description = "Korisnik s ovom vanjskom prijavom već postoji." };

        public override IdentityError InvalidUserName(string? userName)
            => new() { Code = nameof(InvalidUserName), Description = $"Korisničko ime '{userName}' nije ispravno, smije sadržavati samo slova i znamenke." };

        public override IdentityError InvalidEmail(string? email)
            => new() { Code = nameof(InvalidEmail), Description = $"Email '{email}' nije ispravnog formata." };

        public override IdentityError DuplicateUserName(string userName)
            => new() { Code = nameof(DuplicateUserName), Description = $"Korisničko ime '{userName}' je već zauzeto." };

        public override IdentityError DuplicateEmail(string email)
            => new() { Code = nameof(DuplicateEmail), Description = $"Email '{email}' je već registriran." };

        public override IdentityError InvalidRoleName(string? role)
            => new() { Code = nameof(InvalidRoleName), Description = $"Naziv uloge '{role}' nije ispravan." };

        public override IdentityError DuplicateRoleName(string role)
            => new() { Code = nameof(DuplicateRoleName), Description = $"Uloga '{role}' već postoji." };

        public override IdentityError UserAlreadyHasPassword()
            => new() { Code = nameof(UserAlreadyHasPassword), Description = "Korisnik već ima postavljenu lozinku." };

        public override IdentityError UserLockoutNotEnabled()
            => new() { Code = nameof(UserLockoutNotEnabled), Description = "Zaključavanje nije omogućeno za ovog korisnika." };

        public override IdentityError UserAlreadyInRole(string role)
            => new() { Code = nameof(UserAlreadyInRole), Description = $"Korisnik već ima ulogu '{role}'." };

        public override IdentityError UserNotInRole(string role)
            => new() { Code = nameof(UserNotInRole), Description = $"Korisnik nema ulogu '{role}'." };

        public override IdentityError PasswordTooShort(int length)
            => new() { Code = nameof(PasswordTooShort), Description = $"Lozinka mora imati najmanje {length} znakova." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Lozinka mora sadržavati najmanje jedan specijalni znak (npr. !, @, #)." };

        public override IdentityError PasswordRequiresDigit()
            => new() { Code = nameof(PasswordRequiresDigit), Description = "Lozinka mora sadržavati najmanje jednu znamenku ('0'–'9')." };

        public override IdentityError PasswordRequiresLower()
            => new() { Code = nameof(PasswordRequiresLower), Description = "Lozinka mora sadržavati najmanje jedno malo slovo ('a'–'z')." };

        public override IdentityError PasswordRequiresUpper()
            => new() { Code = nameof(PasswordRequiresUpper), Description = "Lozinka mora sadržavati najmanje jedno veliko slovo ('A'–'Z')." };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"Lozinka mora sadržavati najmanje {uniqueChars} različitih znakova." };

        public override IdentityError RecoveryCodeRedemptionFailed()
            => new() { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Iskorištavanje koda za oporavak nije uspjelo." };
    }
}
