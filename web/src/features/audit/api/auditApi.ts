import { keepPreviousData, useMutation, useQuery } from '@tanstack/react-query'
import { downloadBlob, fileNameFrom } from '@/lib/download'
import { http } from '@/lib/http'
import type { AuditFilter, AuditPage } from '../types'

/** Leaves out empty filters so the API applies no condition for them. */
const params = (filter: AuditFilter) => Object.fromEntries(Object.entries(filter).filter(([, v]) => v !== ''))

const auditApi = {
  page: (filter: AuditFilter, page: number) =>
    http.get<AuditPage>('/audit', { params: { ...params(filter), page } }).then((r) => r.data),
  exportExcel: async (filter: AuditFilter) => {
    const response = await http.get<Blob>('/audit/export', { params: params(filter), responseType: 'blob' })
    downloadBlob(fileNameFrom(response.headers['content-disposition'], 'audit-log.xlsx'), response.data)
  },
}

/** Keeps showing the previous page while the next loads, so paging does not flash. */
export function useAuditLog(filter: AuditFilter, page: number) {
  return useQuery({ queryKey: ['audit', filter, page], queryFn: () => auditApi.page(filter, page), placeholderData: keepPreviousData })
}

export function useExportAudit() {
  return useMutation({ mutationFn: auditApi.exportExcel })
}
