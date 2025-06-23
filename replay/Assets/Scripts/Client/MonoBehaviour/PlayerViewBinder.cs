using UnityEngine;

public class PlayerViewBinder : MonoBehaviour, IPlayerViewBinder
{
    public CharacterController move;
    public Animator anim;
    
    public void Move(Vector3 motion)
    {
        move.Move(motion);
    }

    public void PlayAnim(int key, int layer, int normalizedTime)
    {
        anim.Play(key, layer, normalizedTime);
    }

    public void SetAnim(int key, bool value)
    {
        anim.SetBool(key, value);
    }

    public void SetAnim(int key, int value)
    {
        anim.SetInteger(key, value);
    }

    public void SetAnim(int id, float value)
    {
        anim.SetFloat(id, value);
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }
}

public interface IPlayerViewBinder
{
    void Move(Vector3 motion);
    void PlayAnim(int key, int layer, int normalizedTime);
    void SetAnim(int key, bool value);
    void SetAnim(int key, int value);
    void SetAnim(int key, float value);
    Vector3 GetPos();
    Quaternion GetRotation();
    void SetPos(Vector3 pos);
    void SetRotation(Quaternion rot);
}