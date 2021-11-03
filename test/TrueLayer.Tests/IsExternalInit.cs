/*
 * Fix required when targetting .NET Standard 2.1 => https://stackoverflow.com/a/62656145/5524281
 */
using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit {}
}
