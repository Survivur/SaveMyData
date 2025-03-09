using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// ��ü�� �Լ��� �μ��� �����Ͽ� �۾��� �����ϰ� ����� ��ȯ�մϴ�. <br />
/// ��: someCoroutine.Let(StartCoroutine) => StartCoroutine(someCoroutine)
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Extension method to perform an action if the object is not null.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <param name="action">The action to execute if the object is not null.</param>
    public static void Let<T>(this T obj, Action<T> action)
    {
        if (obj != null)
        {
            action(obj);
        }
    }

    /// <summary>
    /// ��ü�� �Լ��� �����Ͽ� Ư�� �۾��� �����ϰ� ����� ��ȯ�մϴ�.
    /// </summary>
    /// <typeparam name="T">�Է� ��ü�� Ÿ��</typeparam>
    /// <typeparam name="TResult">��� ��ü�� Ÿ��</typeparam>
    /// <param name="obj">�۾��� ��� ��ü</param>
    /// <param name="block">�۾��� �����ϴ� �Լ�</param>
    /// <returns>�Լ� ������ ���</returns>
    /// <exception cref="ArgumentNullException">block �Լ��� null�� ��</exception>
    public static TResult Let<T, TResult>(this T obj, Func<T, TResult> block)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        return block(obj);
    }

    /// <summary>
    ///condition�� true�� �� value�� target�� �Ҵ��մϴ�.
    /// </summary>
    /// <typeparam name="T">�Է� ��ü�� Ÿ��</typeparam>
    /// <param name="target">�۾��� ��� ��ü</param>
    /// <param name="condition">����</param>
    /// <param name="value">�ٲ� ��</param>
    public static void SetIfTrue<T>(this T target, bool condition, T value) where T : class
    {
        target = condition ? value : target;
    }

    /// <summary>
    ///condition�� true�� �� value�� target�� �Ҵ��մϴ�.(struct ����)
    /// </summary>
    /// <typeparam name="T">�Է� ��ü�� Ÿ��</typeparam>
    /// <param name="target">�۾��� ��� ��ü</param>
    /// <param name="condition">����</param>
    /// <param name="value">�ٲ� ��</param>
    public static void SetIfTrue<T>(this ref T target, bool condition, T value) where T : struct
    {
        target = condition ? value : target;
    }

    /// <summary>
    /// value�� True�� �� 1f�� ��ȯ�ϰ�, False�� �� -1f�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="goRight"></param>
    /// <returns></returns>
    public static float BoolToSign(this bool value)
    {
        return value ? 1f : -1f;
    }

    /// <summary>
    /// vector2�� x���� �����մϴ�.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="x"></param>
    public static void ChangeX(this ref Vector2 target, float x)
    {
        target = new Vector2(x, target.y);
    }

    /// <summary>
    /// vector2�� y���� �����մϴ�.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="y"></param>
    public static void ChangeY(this ref Vector2 target, float y)
    {
        target = new Vector2(target.x, y);
    }

    /// <summary>
    /// vector3�� x���� �����մϴ�.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="x"></param>
    public static void ChangeX(this ref Vector3 target, float x)
    {
        target = new Vector3(x, target.y, target.z);
    }

    /// <summary>
    /// vector3�� y���� �����մϴ�.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="y"></param>
    public static void ChangeY(this ref Vector3 target, float y)
    {
        target = new Vector3(target.x, y, target.z);        
    }

    /// <summary>
    /// vector3�� z���� �����մϴ�.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="z"></param>
    public static void ChangeZ(this ref Vector3 target, float z)
    {
        target = new Vector3(target.x, target.y, z);
    }
}
