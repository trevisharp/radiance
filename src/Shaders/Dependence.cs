using System;
using System.Text;
using System.Collections.Generic;

public abstract class Dependence
{
    public virtual void AddCode(StringBuilder sb) { }
    public virtual void AddVertexCode(StringBuilder sb) { }
    public virtual void AddFragmentCode(StringBuilder sb) { }
    public virtual void AddHeader(StringBuilder sb) { }
    public virtual void AddVertexHeader(StringBuilder sb) { }
    public virtual void AddFragmentHeader(StringBuilder sb) { }
    public virtual IEnumerable<Action> AddOperations() { yield break; }
    public virtual IEnumerable<Action> AddVertexOperations() { yield break; }
    public virtual IEnumerable<Action> AddFragmentOperations() { yield break; }
}