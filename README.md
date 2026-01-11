[![Open in Visual Studio Code](https://classroom.github.com/assets/open-in-vscode-2e0aaae1b6195c2367325f4f02e2d04e9abb55f0b24a779b69b11b9e10269abc.svg)](https://classroom.github.com/online_ide?assignment_repo_id=22157903&assignment_repo_type=AssignmentRepo)

![라이더 자동생성 다이어그램](<Type Dependencies Diagram for Program and other elements.png>)

최종 작업 전에는 메모장으로 사용 예정

- 기본적 맵 이동 및 적 조우 및 전투 구현 시도.
- 적 조우시 전투씬으로 이동 및 전투.
- 전투 종료시 기존 맵으로 다시 전환

## 조작법 정라
- 화살표: 이동
- Enter: 결정
- I: 인벤토라
- L: 다버그
- T: 임시 체력 삭제 버튼

## 작업 노트

타이틀 제목 꾸미기.  
메뉴 꾸미기  

- TextExtensions의 출력 기능에 널러블(Nullable) 허용해 컬러값을 넣는 경우와 안넣는 경우 다 허용 가능하게 해서 사용 편의성 높임.

- AsciiArtData 클래스 제작
  - AsciiArt 구조체 만들어서 
  - 데이터 정리 및 아스키 
  - 데이터의 정보 값 접근 가능

- TextExtensions에 아스키 Draw 메서드 추가

- TitleScene의 Render 개선 및 커스텀
  - 게임 이름 외각선 설정
  - 로고가 올 위치 중앙에 타이틀 위치 설정
  - 게임 이름 외곽선 그리기
  - 게임 이름 출력(외곽선 안쪽)
  - 로고 외곽선 출력
  - 아스키 데이터에서 로고 데이터 불러와서 그리기
  - 메뉴 출력(로고 아래쪽 중앙 정렬)
    - _logoOutline.X + (_logoOutline.Width / 2) - (_titleOutline.Width / 2)

- Ractangle에 외부 호출시 요소 설정의 번거로움 줄이기 위한 오버라이드 추가.
  - 사용을 시도해보았지만 위치정렬을 위한 계산 과정에서 서로의 값을 참조할 경우 불편이 생겨 추후 필요한 경우에 사용하기로하고 안씀 

- 크레딧창 구현
  - Console.ReadKey(true)를 통해 메인 루프에 들어가 크레딧 창이 지워지는 것을 방지

- 키보드 연속 입력시 화면 렌더링 지연을 방지하기 위해 입력 딜레이 구현 추가(InputManager)
  - Thread.Sleep 이용
  - 시도해보니 GameManager Run 루프에서 Clear 호출이 문제됨.
- 해결 방법 접근
  - Console.Clear() 제거:
    - 각 씬(Scene)의 Render 메서드 안에서 화면을 지워야 할 때만 Clear()를 호출하도록 책임을 넘김.
  - 순서 변경 (Input → Update → Render)
    - 기존: 그리기 → 입력 → 처리
    - 변경: 입력 → 처리 → 그리기
- 구현 방식
  - 화면 전체를 지우지 말고, 변하는 숫자나 텍스트만 커서를 이동해서 그 위에 덮어쓰는 방식으로 구현
  - 씬 이 변했는지 체크하고 변했으면 다시 그려줌
  - 메인 루프에서 클리어 대신 일단 필수적으로 씬에 들어갈때 한번 클리어.
    ```cs
    public virtual void Enter()
    {
        Console.Clear();
    }
    ```
  - Enter 메서드 추상 메서드에서 가상 메서드로 변경
    - 오버라이딩 Enter에 base.Enter(); 한줄씩 추가.
  - 1초에 30fps로 제한 WAIT_TICK = 1000 / 30
- 무한 입력은 입력 딜레이 방식으로 해결되었지만 30 fps라도 플리커링 현상은 여전함
  - 해결 방안: 입력이 들어올때만 Render를 실행할 수 있게 개선. (**"Dirty Flag 패턴"**)
    - 키입력을 씬 클래스 차원에서 체크하고 키입력이 있을때 렌더링 실행하기.
    ```cs
    // InputManager에서 키입력이 있었는지 체크용 변수
    public static bool HasInput => Console.KeyAvailable;
    // 다시 그려줄 필요가 있는지 체크용 변수
    protected bool NeedsRedraw { get; set; } = true;
    // Update에서 키 입력시 true 그리기 끝나면 false
    ```
- 결과는 성공적이지만 키를 꾹 눌렀을때 입력이 처리보다 빨라 밀리는 현상 발생
  - 해결 방안: 키입력을 체크하는 GetUserInput에서 딜레이안에 들어온 키는 버리고 코드의 마지막 단계에서 입력 한번에 쓰인 값을 제외하고 뒤에 쌓인 키들 버려서 중복 입력을 원천 차단한다.
    ```cs
    // 마지막 입력 후 InputDelay(0.1초)가 안 지났으면 입력 무시
    if ((DateTime.Now - _lastInputTime).TotalMilliseconds < InputDelay)
    {
        // 입력 버퍼 비우기 (중복 입력 방지)
        while (Console.KeyAvailable) Console.ReadKey(true); 
        return;
    }
    // 중복 입력 원천 차단. 입력 한번에 쓰인 값을 제외하고 뒤에 쌓인 키들 버리기.
    while (Console.KeyAvailable)
    {
        Console.ReadKey(true);
    }
    ```
    이렇게 하면 입력이 끝낱을때 바로 동작이 멈추는걸 확인 가능했다.
- Read