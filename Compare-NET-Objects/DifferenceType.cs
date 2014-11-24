using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KellermanSoftware.CompareNetObjects
{
    /// <summary>
    /// The type of difference
    /// </summary>
    public enum DifferenceType
    {
        /// <summary>
        /// Regular value mismatch
        /// </summary>
        Mismatch = 0,
        /// <summary>
        /// Missing value when using MissingAndMisordered logic
        /// </summary>
        Missing = 1,
        /// <summary>
        /// Misordered value when using MissingAndMisordered logic
        /// </summary>
        Misorder = 2,
        /// <summary>
        /// Exception that occured during property accessing
        /// </summary>
        Exception = 3,
        /// <summary>
        /// Not implemented exception that occured during property accessing
        /// </summary>
        NotImplemented = 4
    }
}
