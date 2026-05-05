using System;
using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global

namespace Scanii
{
  public class ScaniiTarget
  {
    /// <summary>
    /// Latency-routed endpoint. Routes to the nearest region automatically,
    /// but does not guarantee which region processes your data.
    /// </summary>
    /// <remarks>
    /// Deprecated: use an explicit regional target for data residency compliance —
    /// <see cref="Us1"/>, <see cref="Eu1"/>, <see cref="Eu2"/>,
    /// <see cref="Ap1"/>, <see cref="Ap2"/>, <see cref="Ca1"/>.
    /// Will be removed in a future major version.
    /// </remarks>
    [Obsolete("Use an explicit regional target for data residency compliance: Us1, Eu1, Eu2, Ap1, Ap2, Ca1. Will be removed in a future major version.")]
    public static readonly ScaniiTarget Auto = new ScaniiTarget("https://api.scanii.com");
    public static readonly ScaniiTarget Us1 = new ScaniiTarget("https://api-us1.scanii.com");
    public static readonly ScaniiTarget Eu1 = new ScaniiTarget("https://api-eu1.scanii.com");
    public static readonly ScaniiTarget Eu2 = new ScaniiTarget("https://api-eu2.scanii.com");
    public static readonly ScaniiTarget Ap1 = new ScaniiTarget("https://api-ap1.scanii.com");
    public static readonly ScaniiTarget Ap2 = new ScaniiTarget("https://api-ap2.scanii.com");
    public static readonly ScaniiTarget Ca1 = new ScaniiTarget("https://api-ca1.scanii.com");

    // ReSharper disable once MemberCanBePrivate.Global
    public ScaniiTarget(string endpoint)
    {
      Endpoint = new Uri(endpoint);
    }

    public Uri Endpoint { get; }

    public static IEnumerable<ScaniiTarget> All()
    {
      return new List<ScaniiTarget>
      {
        Auto, Ap1, Ap2, Us1, Eu1, Eu2, Ca1
      };
    }
  }
}
