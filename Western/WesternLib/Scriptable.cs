using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{


    public class Scriptable
    {
        private static ScriptEngine _engine = null;

        private static ScriptScope _scope = null;
        private Dictionary<string, CompiledCode> _scripts = new Dictionary<string, CompiledCode>();
        ScriptBindings _bindings = new ScriptBindings();
        protected ScriptScope Scope { get { return _scope; } }

        public Scriptable()
        {
            InitializeScriptEngine();
            _scope = _engine.CreateScope();
            _scope.SetVariable("Game", _bindings);
        }

        protected void AddScript(string name, string source)
        {
            try
            {
                ScriptSource sc = _engine.CreateScriptSourceFromString(source, SourceCodeKind.Statements);
                CompiledCode cc = sc.Compile();
                _scripts.Add(name, cc);
            }
            catch (Exception ex)
            {
                Console.Write("Script " + name + " failed to execute. " + ex.ToString());   
            }
        }

        protected void Execute(string name, Dictionary<string, string> parameters)
        {
            //Console.WriteLine("Executing: " + name);
            _scope.SetVariable("Parameters", parameters);
            try
            {
                dynamic returnVal = _scripts[name].Execute(_scope);
            }
            catch(Exception ex)
            {
                Console.Write("Script "  + name +" failed to execute. " + ex.ToString());
            }
            
            return;
        }

        private static void InitializeScriptEngine()
        {
            if (_engine == null)
                _engine = Python.CreateEngine();
        }

    }

}
