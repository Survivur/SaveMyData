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
                // ����, ������Ʈ�� ��ġ�� �ٲ��, �� �ڵ嵵 �����ؾ� �մϴ�.
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
