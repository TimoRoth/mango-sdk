using System;
using System.Diagnostics;

namespace Mango.Compiler.Diagnostics
{
    public abstract class Diagnostic : IEquatable<Diagnostic>
    {
        private protected Diagnostic() { }

        public abstract DiagnosticSeverity Severity { get; }

        public sealed override bool Equals(object obj) => obj is Diagnostic other && Equals(other);

        public abstract bool Equals(Diagnostic other);

        public abstract override int GetHashCode();
    }

    internal class DiagnosticInfo
    {
        private readonly int _errorCode;
        private readonly DiagnosticSeverity _severity;

        internal DiagnosticInfo(ErrorCode errorCode, DiagnosticSeverity severity)
        {
            _errorCode = (int)errorCode;
            _severity = severity;
        }

        public int ErrorCode => _errorCode;

        public DiagnosticSeverity Severity => _severity;
    }

    internal class DiagnosticWithInfo : Diagnostic
    {
        private readonly DiagnosticInfo _info;

        internal DiagnosticWithInfo(DiagnosticInfo info)
        {
            Debug.Assert(info != null);

            _info = info;
        }

        public DiagnosticInfo Info => _info;

        public override DiagnosticSeverity Severity => _info.Severity;

        public override bool Equals(Diagnostic other) => (object)this == other || other is DiagnosticWithInfo diagnostic && _info.Equals(diagnostic.Info);

        public override int GetHashCode() => _info.GetHashCode();

        public override string ToString() => (_info.Severity == DiagnosticSeverity.Error ? "error" : "warning") + " MANGO" + _info.ErrorCode.ToString("D4") + ": " + ((ErrorCode)_info.ErrorCode).ToString();
    }
}
