// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace MobileSupport
{
    public readonly struct ThermalStatus
    {
        public bool IsIOS => !_isAndroid;
        public bool IsAndroid => _isAndroid;
        
        private readonly bool _isAndroid;
        private readonly int _status;
        
        public ThermalStatus(ThermalStatusAndroid status)
        {
            _isAndroid = true;
            _status = (int)status;
        }
        
        public ThermalStatus(ThermalStatusIOS status)
        {
            _isAndroid = false;
            _status = (int)status;
        }
        
        public static implicit operator ThermalStatus(ThermalStatusAndroid status) => new (status);
        
        public static implicit operator ThermalStatus(ThermalStatusIOS status) => new (status);

        public ThermalStatusAndroid UnwrapAsAndroid()
        {
            if(!_isAndroid) throw new InvalidOperationException("This status is not for Android.");
            return (ThermalStatusAndroid)_status;
        }
        
        public ThermalStatusIOS UnwrapAsIOS()
        {
            if(_isAndroid) throw new InvalidOperationException("This status is not for iOS.");
            return (ThermalStatusIOS)_status;
        }
    }

    public enum ThermalStatusAndroid
    {
        None,
        Light,
        Moderate,
        Severe,
        Critical,
        Emergency,
        Shutdown,
    }

    public enum ThermalStatusIOS
    {
        Nominal,
        Fair,
        Serious,
        Critical
    }
}
