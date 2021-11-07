using UnityEngine;


[CreateAssetMenu(fileName = "__baseTool", menuName = "On Camera Draw Objects/Base Tool")]
public class OCD_BaseDraw : ScriptableObject
{
    public virtual string getName() { return "Look around"; }
    public virtual void draw(Camera camera, Transform cursor, object[] args) { }
    public virtual void onSelect(Camera camera, Transform cursor, object[] args) { }
    public virtual void onDeselect(Camera camera, Transform cursor, object[] args) { }
}
