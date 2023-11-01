using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[AddComponentMenu("Custom/InteractObject")]
public class InteractObject : MonoBehaviour
{
    [SerializeField] eInteract type;
    public eInteract InteractType { get { return type; } }
}
