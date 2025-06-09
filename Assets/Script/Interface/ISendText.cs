using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISendText
{
    public List<Func<string>> TextFunc { get; }
}
