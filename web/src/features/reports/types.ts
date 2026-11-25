/** Mirrors Application/Common/Reports/ReportColumnKind. */
export type ReportColumnKind = 'Text' | 'Number' | 'Date' | 'Time' | 'Percent'

/** Mirrors Application/Common/Reports/ReportTable. Each row has one value per column. */
export interface ReportTable {
  title: string
  subtitle: string
  fileStem: string
  columns: { header: string; kind: ReportColumnKind }[]
  rows: (string | number | boolean | null)[][]
}

/** Mirrors Features/Reports/Queries/GetReportOptions/ReportOptionsDto. */
export interface ReportOptions {
  venues: { id: number; name: string }[]
  cohorts: { programme: string; academicYear: number }[]
  events: { id: number; name: string }[]
}

/** Query parameters sent to a report endpoint; empty values are left out. */
export type ReportParams = Record<string, string>
