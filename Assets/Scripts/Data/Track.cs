using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track {
  private Mesh trackMesh, innerBarrier, outerBarrier;

  bool isActive;

  public Mesh TrackMesh { get; set; }
  public Mesh InnerBarrier { get; set; }
  public Mesh OuterBarrier { get; set; }

  public bool IsActive { get; set; }

}