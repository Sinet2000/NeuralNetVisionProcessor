using System.ComponentModel.DataAnnotations;
using Bogus;

namespace NetVisionProc.Common.Data.BogusUtil
{
    public static class BogusFakerExt
    {
        public static T GetRandomItem<T>(this IEnumerable<T> items)
        {
            Guard.Against.NullOrEmpty(items, nameof(items));

            int maxIndex = items.Count() - 1;
            if (maxIndex == 0)
            {
                return items.First();
            }

            var index = new Random().Next(0, maxIndex);

            return items.ElementAt(index);
        }

        public static string PersonalCode(this Person person)
        {
            return $"{person.DateOfBirth.ToString("ddMMyy", null)}-{person.Random.RandomNumberString(5)}";
        }

        public static string OperationStatus(this Bogus.Faker faker)
        {
            var operation = faker.PickRandom<OperationType>();
            return operation.GetDisplayName() ?? string.Empty;
        }

        public static string RandomNumberString(this Randomizer fake, int length)
        {
            var number = new char[length];

            for (int i = 0; i < length; i++)
            {
                int digit = fake.Number(9);
                number[i] = (char)(digit + 48);
            }

            return new string(number);
        }

        private enum OperationType : byte
        {
            [Display(Name = "In Progress")]
            InProgress = 0,
    
            [Display(Name = "Active")]
            Active = 1,
    
            [Display(Name = "Pending")]
            Pending = 2,
    
            [Display(Name = "Cancelled")]
            Cancelled = 3,
    
            [Display(Name = "Completed")]
            Completed = 4,
    
            [Display(Name = "Failed")]
            Failed = 5,
    
            [Display(Name = "Paused")]
            Paused = 6,
    
            [Display(Name = "Scheduled")]
            Scheduled = 7,
    
            [Display(Name = "Aborted")]
            Aborted = 8,
    
            [Display(Name = "Queued")]
            Queued = 9
        }
    }
}
