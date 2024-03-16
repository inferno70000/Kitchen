using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgressBar
{
    public event EventHandler<OnProgressChangedEventAgr> OnProgressChanged;
    public class OnProgressChangedEventAgr : EventArgs
    {
        public float progressNomalized;
    }
}
