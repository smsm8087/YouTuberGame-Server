# 유튜버 키우기 (Idle YouTuber Tycoon) - Server & Admin
## 작업 규칙
1. 코드 작업 완료 후 CLAUDE.md "현재 상태" 섹션 업데이트
2. 새 파일 생성 시 해당 파일의 역할을 간단히 기록
3. 구조 변경 시 "변경 이력"에 날짜 + 이유 기록
4. 커밋 메시지는 한국어로, 무엇을 왜 했는지 명확하게
5. TODO 항목 완료 시 상태를 "완료"로 변경

## 변경 이력
- 2026-02-10: 프로젝트 초기 구조 생성 (Sonnet)
- 2026-02-11: CLAUDE.md 추가 (Opus)
- 2026-02-11: Phase 1 캐릭터 레벨업/돌파 시스템 구현 (Opus)
- 2026-02-11: Phase 2 콘텐츠 시스템 구현 (게임 핵심 루프) (Opus)
- 2026-02-11: Phase 3 장비 시스템 구현 (Opus)

---
## 프로젝트 개요
- **프레임워크**: ASP.NET Core 9.0
- **DB**: MySQL (Pomelo EF Core)
- **인증**: JWT Bearer + BCrypt
- **어드민**: Blazor Server + Bootstrap 5
- **호스팅**: Oracle Cloud 무료 티어 (DAU 5,000 이상 시 이사 고려)
- **클라이언트 레포**: https://github.com/smsm8087/YouTuberGame

## 아키텍처
YouTuberGame.sln
├── YouTuberGame.API          # REST API 서버
├── YouTuberGame.Admin        # Blazor 어드민 웹
└── YouTuberGame.Shared       # 공유 모델/DTO



## 서버 원칙
- **서버 권위 모델**: 재화 변경, 가챠, 결제 등 중요 로직은 반드시 서버에서 처리
- **클라이언트는 표시만**: 클라가 보낸 데이터를 그대로 신뢰하지 않음
- **API URL**: 클라이언트에서 하드코딩 금지 (Config로 관리)

## API 엔드포인트 (구현 상태)
| 엔드포인트 | 상태 | 설명 |
|-----------|------|------|
| POST /api/auth/register | 완료 | 회원가입 |
| POST /api/auth/login | 완료 | 로그인 (JWT 발급) |
| GET /api/player/me | 완료 | 플레이어 정보 조회 |
| PUT /api/player/save | 완료 | 플레이어 데이터 저장 |
| POST /api/gacha/draw | 완료 | 가챠 뽑기 |
| GET /api/characters | 완료 | 전체 캐릭터 목록 (도감) |
| POST /api/player/characters/{id}/levelup | 완료 | 캐릭터 레벨업 (ExpChips 소모) |
| POST /api/player/characters/{id}/breakthrough | 완료 | 캐릭터 돌파 (중복 카드 소모) |
| POST /api/content/start | 완료 | 콘텐츠 제작 시작 (캐릭터 배치, 타이머) |
| GET /api/content/producing | 완료 | 제작 중 콘텐츠 조회 (남은 시간) |
| POST /api/content/{id}/complete | 완료 | 제작 완료 처리 |
| POST /api/content/{id}/upload | 완료 | 콘텐츠 업로드 (조회수/수익 시뮬레이션) |
| GET /api/content/history | 완료 | 업로드 히스토리 |
| GET /api/player/equipment | 완료 | 장비 조회 (4종류) |
| POST /api/player/equipment/{type}/upgrade | 완료 | 장비 업그레이드 (Gold 소모) |
| POST /api/player/studio/upgrade | 미구현 | 스튜디오 업그레이드 |
| GET /api/rankings/weekly | 미구현 | 주간 랭킹 |
| GET /api/trend/today | 미구현 | 오늘의 트렌드 |
| POST /api/purchase/verify | 미구현 | 결제 검증 |

## 어드민 웹 기능 (TODO)
- 공지사항 관리
- FAQ 관리
- 점검 ON/OFF 토글
- 유저 조회/밴
- 마스터데이터 업로드 (JSON)
- 통계 대시보드: 매출(일/주/월, ARPU, ARPPU), 유저(DAU/MAU, 리텐션), 게임내(가챠, 재화 분포)
- 결제 내역 추적

## 알림 시스템 (TODO)
- Discord Webhook 연동
- 채널: #서버-에러, #결제-알림, #일일-리포트, #신규-가입

## 운영 기능 (TODO)
- 데이터 핫 업데이트: 마스터데이터 버전 관리 → 클라 시작 시 체크
- 무중단 배포 (Blue-Green)
- 점검 모드: 어드민에서 토글 → 클라에서 "점검 중" 팝업
- 일일 자동 백업
- 로그 수집 (Serilog → 파일, 추후 ELK 고려)

## 수익 모델 상품
- 광고 제거권 $4.99 (1회)
- 월간 구독 $2.99/월
- 스타터 패키지 $4.99 (1회)
- 보석: $0.99(100개), $4.99(550개), $9.99(1200개)

## 현재 상태 (2026-02-11)
- JWT 인증 (회원가입/로그인) 구현 완료
- 플레이어 데이터 CRUD 구현 완료
- 가챠 뽑기 구현 완료
- 캐릭터 레벨업/돌파 시스템 구현 완료
- 콘텐츠 제작/업로드 시스템 구현 완료 (핵심 게임 루프)
- **장비 시스템 구현 완료**
- DB 마이그레이션: InitialCreate, AddContentSystem, AddEquipmentSystem
- 어드민은 Blazor 템플릿 상태 (커스텀 페이지 없음)
- Discord Webhook 미연동
- 스튜디오/랭킹 API 미구현

### 새로 추가된 파일 (Phase 1)
- `YouTuberGame.Shared/DTOs/CharacterDTOs.cs` - 레벨업/돌파 DTO
- `YouTuberGame.API/Services/CharacterService.cs` - 캐릭터 성장 로직
- `YouTuberGame.API/Controllers/CharacterController.cs` - 캐릭터 API

### 새로 추가된 파일 (Phase 2)
- `YouTuberGame.Shared/Models/Content.cs` - 콘텐츠 엔티티 (Status, Genre, 품질, 타이머)
- `YouTuberGame.Shared/DTOs/ContentDTOs.cs` - 콘텐츠 제작/업로드 DTO
- `YouTuberGame.API/Services/ContentService.cs` - 콘텐츠 제작 로직 (실시간 타이머, 품질 계산, 수익 시뮬레이션)
- `YouTuberGame.API/Controllers/ContentController.cs` - 콘텐츠 API

### 새로 추가된 파일 (Phase 3)
- `YouTuberGame.Shared/Models/Equipment.cs` - 장비 엔티티 (4종류)
- `YouTuberGame.Shared/DTOs/EquipmentDTOs.cs` - 장비 조회/업그레이드 DTO
- `YouTuberGame.API/Services/EquipmentService.cs` - 장비 로직 (업그레이드 비용 Level*500, 보너스 Level*5)
- `YouTuberGame.API/Controllers/EquipmentController.cs` - 장비 API

## 개발 환경 설정
```bash
# DB 생성
mysql -u root -p -e "CREATE DATABASE youtubergame;"

# 마이그레이션 적용
cd src/YouTuberGame.API
dotnet ef database update

# 서버 실행
dotnet run

# Swagger: https://localhost:5001/swagger

참고 문서
API 명세: (클라 레포) docs/API.md
DB 스키마: (클라 레포) docs/DATABASE.md
