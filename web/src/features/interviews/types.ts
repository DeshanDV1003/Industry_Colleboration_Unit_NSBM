export type InterviewStage = 'SoftSkills' | 'Technical'
export type AnswerInputMode = 'Typed' | 'Voice'

/** Mirrors Features/Interviews/Dtos/AnswerFeedbackDto. */
export interface AnswerFeedback {
  answerId: number
  text: string
  inputMode: AnswerInputMode
  answeredAt: string
  wordCount: number
  matchedTopics: string[]
  topicsToConsider: string[]
  coveragePercent: number
  notes: string[]
}

export interface InterviewQuestion {
  id: number
  topic: string
  prompt: string
  tip: string
  latestAnswer: AnswerFeedback | null
  followUps: InterviewQuestion[]
}

/** Mirrors Features/Interviews/Dtos/InterviewSessionDto. */
export interface InterviewSession {
  stage: InterviewStage
  careerPathId: number | null
  careerPathName: string | null
  questions: InterviewQuestion[]
}

export interface ReportItem {
  stage: InterviewStage
  careerPathName: string | null
  topic: string
  prompt: string
  isFollowUp: boolean
  answer: AnswerFeedback
}

/** Mirrors Features/Interviews/Dtos/InterviewReportDto. */
export interface InterviewReport {
  studentName: string
  studentNo: string
  generatedAt: string
  items: ReportItem[]
}
