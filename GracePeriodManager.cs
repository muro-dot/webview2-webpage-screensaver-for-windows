using System;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 화면보호기 실행 초기 유예시간(Grace Period)을 중앙에서 단일 관리하는 매니저입니다.
    /// 탐색기에서 우클릭 '테스트'나 실행 시 발생하는 잔여 마우스/키보드 입력으로 인해
    /// 화면보호기가 켜지자마자 즉시 꺼지는 문제를 방지하기 위해 1.5초의 보호 기간을 전역 통일합니다.
    /// </summary>
    public static class GracePeriodManager
    {
        // 화면보호기 프로세스가 시작된 기준 시각 (단일 진실 공급원)
        private static readonly DateTime AppStartTime = DateTime.Now;

        // 유예시간 기준 (초 단위, 1.5초)
        public const double GracePeriodSeconds = 1.5;

        /// <summary>
        /// 현재 시각이 시작 후 1.5초 유예시간 이내인지 여부를 반환합니다.
        /// 마우스 클릭, 웹뷰 키 입력, 저수준 훅, 마우스 이동 감지 등 모든 입력 종료 경로에서 공통 호출합니다.
        /// </summary>
        public static bool IsInGracePeriod()
        {
            return DateTime.Now < AppStartTime.AddSeconds(GracePeriodSeconds);
        }
    }
}
