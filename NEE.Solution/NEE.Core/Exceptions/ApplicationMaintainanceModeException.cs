using System;

namespace NEE.Core.Exceptions
{
    public class ApplicationMaintainanceModeException : Exception
    {
        public ApplicationMaintainanceModeException()
            : base($"Εκτελούνται ενέργειες συντήρησης της εφαρμογής. Δεν μπορούν να γίνουν ενημερώσεις αυτή τη στιγμή. Παρακαλώ προσπαθήστε αργότερα.")
        { }

    }
}
