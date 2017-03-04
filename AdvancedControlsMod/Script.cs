using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Lench.AdvancedControls
{
    public class Script
    {
        public static Script Global => _global ?? (_global = new Script());

        private static Script _global;
        private static ScriptEngine _scriptEngine;

        private ScriptScope _scriptScope;

        public Script()
        {
            if (_scriptEngine == null)
                _scriptEngine = Python.CreateEngine();

            _scriptScope = _scriptEngine.CreateScope(new Dictionary<string, object>
            {
                {"machine", Machine.Current }
            });
        }

        public object Execute(string expression)
        {
            return _scriptEngine.Execute(expression, _scriptScope);
        }

        internal static string FormatException(Exception e)
        {
            e = e.InnerException ?? e;
            return _scriptEngine.GetService<ExceptionOperations>().FormatException(e);
        }

        public Func<object> Compile(string initialisationCode)
        {
            throw new NotImplementedException();
        }

        public T GetVariable<T>(object name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable GetVariableNames()
        {
            throw new NotImplementedException();
        }
    }
}
