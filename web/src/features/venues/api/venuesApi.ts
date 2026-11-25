import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { Venue, VenueBooking, VenueBookingInput, VenueBookingOptions, VenueInput } from '../types'

/** Every venue query starts here, so any allocation change refreshes lists, counts and options together. */
const VENUES = ['venues'] as const

const venuesApi = {
  list: () => http.get<Venue[]>('/venues').then((r) => r.data),
  bookings: (from: string, to: string) =>
    http.get<VenueBooking[]>('/venues/bookings', { params: { from, to } }).then((r) => r.data),
  options: () => http.get<VenueBookingOptions>('/venues/booking-options').then((r) => r.data),
  create: (input: VenueInput) => http.post<{ id: number }>('/venues', input).then((r) => r.data),
  update: ({ id, input }: { id: number; input: VenueInput }) => http.put(`/venues/${id}`, input),
  book: (input: VenueBookingInput) => http.post<{ ids: number[] }>('/venues/bookings', input).then((r) => r.data),
  cancel: (id: number) => http.delete(`/venues/bookings/${id}`),
}

export function useVenues() {
  return useQuery({ queryKey: [...VENUES, 'list'], queryFn: venuesApi.list })
}

export function useVenueBookings(from: string, to: string) {
  return useQuery({ queryKey: [...VENUES, 'bookings', from, to], queryFn: () => venuesApi.bookings(from, to) })
}

/** Registrar only; pass enabled=false for read-only viewers. */
export function useBookingOptions(enabled = true) {
  return useQuery({ queryKey: [...VENUES, 'options'], queryFn: venuesApi.options, enabled })
}

function useVenueMutation<T, R>(fn: (input: T) => Promise<R>) {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: fn, onSuccess: () => queryClient.invalidateQueries({ queryKey: VENUES }) })
}

export const useCreateVenue = () => useVenueMutation(venuesApi.create)
export const useUpdateVenue = () => useVenueMutation(venuesApi.update)
export const useBookVenue = () => useVenueMutation(venuesApi.book)
export const useCancelBooking = () => useVenueMutation(venuesApi.cancel)
