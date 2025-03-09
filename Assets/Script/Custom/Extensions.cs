using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 객체를 함수의 인수로 전달하여 작업을 수행하고 결과를 반환합니다. <br />
/// 예: someCoroutine.Let(StartCoroutine) => StartCoroutine(someCoroutine)
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
    /// 객체를 함수에 전달하여 특정 작업을 수행하고 결과를 반환합니다.
    /// </summary>
    /// <typeparam name="T">입력 객체의 타입</typeparam>
    /// <typeparam name="TResult">결과 객체의 타입</typeparam>
    /// <param name="obj">작업의 대상 객체</param>
    /// <param name="block">작업을 수행하는 함수</param>
    /// <returns>함수 실행의 결과</returns>
    /// <exception cref="ArgumentNullException">block 함수가 null일 때</exception>
    public static TResult Let<T, TResult>(this T obj, Func<T, TResult> block)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        return block(obj);
    }

    /// <summary>
    ///condition이 true일 때 value를 target에 할당합니다.
    /// </summary>
    /// <typeparam name="T">입력 객체의 타입</typeparam>
    /// <param name="target">작업의 대상 객체</param>
    /// <param name="condition">조건</param>
    /// <param name="value">바꿀 값</param>
    public static void SetIfTrue<T>(this T target, bool condition, T value) where T : class
    {
        target = condition ? value : target;
    }

    /// <summary>
    ///condition이 true일 때 value를 target에 할당합니다.(struct 전용)
    /// </summary>
    /// <typeparam name="T">입력 객체의 타입</typeparam>
    /// <param name="target">작업의 대상 객체</param>
    /// <param name="condition">조건</param>
    /// <param name="value">바꿀 값</param>
    public static void SetIfTrue<T>(this ref T target, bool condition, T value) where T : struct
    {
        target = condition ? value : target;
    }

    /// <summary>
    /// value가 True일 때 1f을 반환하고, False일 때 -1f을 반환합니다.
    /// </summary>
    /// <param name="goRight"></param>
    /// <returns></returns>
    public static float BoolToSign(this bool value)
    {
        return value ? 1f : -1f;
    }

    /// <summary>
    /// vector2의 x값을 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="x"></param>
    public static void ChangeX(this ref Vector2 target, float x)
    {
        target = new Vector2(x, target.y);
    }

    /// <summary>
    /// vector2의 y값을 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="y"></param>
    public static void ChangeY(this ref Vector2 target, float y)
    {
        target = new Vector2(target.x, y);
    }

    /// <summary>
    /// vector3의 x값을 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="x"></param>
    public static void ChangeX(this ref Vector3 target, float x)
    {
        target = new Vector3(x, target.y, target.z);
    }

    /// <summary>
    /// vector3의 y값을 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="y"></param>
    public static void ChangeY(this ref Vector3 target, float y)
    {
        target = new Vector3(target.x, y, target.z);        
    }

    /// <summary>
    /// vector3의 z값을 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="z"></param>
    public static void ChangeZ(this ref Vector3 target, float z)
    {
        target = new Vector3(target.x, target.y, z);
    }
}
