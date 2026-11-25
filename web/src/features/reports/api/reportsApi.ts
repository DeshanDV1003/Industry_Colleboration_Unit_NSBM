import { useMutation, useQuery } from '@tanstack/react-query'
import { downloadBlob, fileNameFrom } from '@/lib/download'
import { http } from '@/lib/http'
import type { ReportOptions, ReportParams, ReportTable } from '../types'

const REPORTS = ['reports'] as const

/** Drops empty filters so the API applies its defaults. */
const clean = (params: ReportParams) => Object.fromEntries(Object.entries(params).filter(([, v]) => v !== ''))

const reportsApi = {
  options: () => http.get<ReportOptions>('/reports/options').then((r) => r.data),
  table: (endpoint: string, params: ReportParams) =>
    http.get<ReportTable>(`/reports/${endpoint}`, { params: clean(params) }).then((r) => r.data),
  excel: async ({ endpoint, params }: { endpoint: string; params: ReportParams }) => {
    const response = await http.get<Blob>(`/reports/${endpoint}`, { params: { ...clean(params), format: 'xlsx' }, responseType: 'blob' })
    downloadBlob(fileNameFrom(response.headers['content-disposition'], `${endpoint}.xlsx`), response.data)
  },
}

export function useReportOptions() {
  return useQuery({ queryKey: [...REPORTS, 'options'], queryFn: reportsApi.options })
}

/** Loads the preview only when asked (params set), and again whenever the filters change. */
export function useReportTable(endpoint: string, params: ReportParams | null) {
  return useQuery({
    queryKey: [...REPORTS, endpoint, params],
    queryFn: () => reportsApi.table(endpoint, params!),
    enabled: params !== null,
  })
}

export function useDownloadExcel() {
  return useMutation({ mutationFn: reportsApi.excel })
}
