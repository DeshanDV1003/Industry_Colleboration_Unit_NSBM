/** Mirrors Domain.Enums.InterviewStage. */
export type InterviewStage = 'SoftSkills' | 'Technical'
export type AiInterviewStatus = 'InProgress' | 'Completed'
export type AiTurnSpeaker = 'Interviewer' | 'Student'

export interface AiInterviewAvailability { isAvailable: boolean; questionsPerSession: number }

export interface AiInterviewSummary {
  id: number
  stage: InterviewStage
  targetRole: string
  status: AiInterviewStatus
  startedAt: string
  completedAt: string | null
  answerCount: number
  averageScore: number | null
}

export interface AiInterviewTurn { order: number; speaker: AiTurnSpeaker; text: string; createdAt: string }

export interface AiInterviewScore { criterion: string; score: number; comment: string }

export interface AiInterviewFeedback {
  summary: string
  strengths: string[]
  improvements: string[]
  scores: AiInterviewScore[]
  averageScore: number
}

/** Mirrors Features/AiInterviews/Dtos/AiInterviewDto. */
export interface AiInterview {
  id: number
  stage: InterviewStage
  targetRole: string
  status: AiInterviewStatus
  startedAt: string
  completedAt: string | null
  questionsPerSession: number
  questionCount: number
  /** The last message is an unanswered question. */
  awaitingAnswer: boolean
  /** The last message is an answer the AI has not replied to yet (after a service error). */
  needsNextStep: boolean
  turns: AiInterviewTurn[]
  feedback: AiInterviewFeedback | null
}
