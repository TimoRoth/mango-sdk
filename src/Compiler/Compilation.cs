using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Mango.Compiler.Binding;
using Mango.Compiler.Syntax;

namespace Mango.Compiler
{
    public sealed class Compilation
    {
        private readonly string _applicationName;
        private readonly ImmutableHashSet<SyntaxTree> _syntaxTrees;

        private Binder _binder;

        private Compilation(string applicationName, ImmutableHashSet<SyntaxTree> syntaxTrees)
        {
            _applicationName = applicationName;
            _syntaxTrees = syntaxTrees;
        }

        public string ApplicationName => _applicationName;

        public IEnumerable<SyntaxTree> SyntaxTrees => _syntaxTrees;

        internal Binder Binder => GetBinder();

        public static Compilation Create(string applicationName, IEnumerable<SyntaxTree> syntaxTrees = null)
        {
            var compilation = new Compilation(applicationName, ImmutableHashSet<SyntaxTree>.Empty);

            if (syntaxTrees != null)
            {
                compilation = compilation.AddSyntaxTrees(syntaxTrees);
            }

            return compilation;
        }

        public Compilation AddSyntaxTrees(params SyntaxTree[] syntaxTrees)
        {
            return AddSyntaxTrees((IEnumerable<SyntaxTree>)syntaxTrees);
        }

        public Compilation AddSyntaxTrees(IEnumerable<SyntaxTree> syntaxTrees)
        {
            if (syntaxTrees == null)
            {
                throw new ArgumentNullException(nameof(syntaxTrees));
            }

            return Update(_applicationName, _syntaxTrees.Union(syntaxTrees));
        }

        public Emit.CompiledModules Build()
        {
            return Compiler.Emit.CompiledModules.Create(this);
        }

        public bool ContainsSyntaxTree(SyntaxTree syntaxTree)
        {
            return _syntaxTrees.Contains(syntaxTree);
        }

        public void Emit(string path)
        {
            Build().Emit(path);
        }

        public Analysis.SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
        {
            return new Analysis.SemanticModel(this);
        }

        public Compilation RemoveAllSyntaxTrees()
        {
            return Update(_applicationName, ImmutableHashSet<SyntaxTree>.Empty);
        }

        public Compilation RemoveSyntaxTrees(params SyntaxTree[] syntaxTrees)
        {
            return RemoveSyntaxTrees((IEnumerable<SyntaxTree>)syntaxTrees);
        }

        public Compilation RemoveSyntaxTrees(IEnumerable<SyntaxTree> syntaxTrees)
        {
            if (syntaxTrees == null)
            {
                throw new ArgumentNullException(nameof(syntaxTrees));
            }

            return Update(_applicationName, _syntaxTrees.Except(syntaxTrees));
        }

        public Compilation ReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree newTree)
        {
            if (oldTree == null)
            {
                throw new ArgumentNullException(nameof(oldTree));
            }
            else if (newTree == oldTree)
            {
                return this;
            }
            else if (newTree == null)
            {
                return Update(_applicationName, _syntaxTrees.Remove(oldTree));
            }
            else
            {
                return Update(_applicationName, _syntaxTrees.Remove(oldTree).Add(newTree));
            }
        }

        public Compilation WithApplicationName(string applicationName)
        {
            return Update(applicationName, _syntaxTrees);
        }

        private Binder GetBinder()
        {
            if (_binder == null)
            {
                var binder = new Binder(this);

                Interlocked.CompareExchange(ref _binder, binder, null);
            }

            return _binder;
        }

        private Compilation Update(string applicationName, ImmutableHashSet<SyntaxTree> syntaxTrees)
        {
            if ((object)_applicationName == applicationName && (object)_syntaxTrees == syntaxTrees)
            {
                return this;
            }
            else
            {
                return new Compilation(applicationName, syntaxTrees);
            }
        }
    }
}
