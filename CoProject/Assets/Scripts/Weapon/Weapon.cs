using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private Transform h;
    [SerializeField] private Transform v;

    protected PlayerDirection direction;

    public abstract void Attack();
    public void SetDirection(PlayerDirection playerDirection) => direction = playerDirection;

    protected virtual void Update()
    {
        h.gameObject.SetActive(direction is PlayerDirection.left or PlayerDirection.right);
        h.localScale = new Vector3(direction is PlayerDirection.left ? -1 : 1, 1, 1);

        v.gameObject.SetActive(direction is PlayerDirection.up or PlayerDirection.down);
        v.localScale = new Vector3(direction is PlayerDirection.down ? -1 : 1, 1, 1);
    }
}
