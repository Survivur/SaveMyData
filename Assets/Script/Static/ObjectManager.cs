using UnityEngine;

public static class ObjectManager
{
    private static GameObject _bulletManager;
    public static GameObject BulletManager
    {
        get
        {
            if (_bulletManager == null)
            {
                // 만약, 오브젝트의 위치가 바뀌면, 이 코드도 수정해야 합니다.
                _bulletManager = GameObject.FindWithTag(nameof(BulletManager));
                if (_bulletManager == null)
                {
                    Debug.LogError($"{nameof(BulletManager)} GameObject not found!");
                }
            }
            return _bulletManager;
        }
    }
}
