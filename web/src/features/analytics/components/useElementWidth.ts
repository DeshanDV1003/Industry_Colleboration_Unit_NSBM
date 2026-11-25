import { useEffect, useRef, useState } from 'react'

/** The rendered width of an element, kept up to date, so SVG charts draw at real pixels (text never stretches). */
export function useElementWidth<T extends HTMLElement>() {
  const ref = useRef<T>(null)
  const [width, setWidth] = useState(0)

  useEffect(() => {
    if (!ref.current) return
    const observer = new ResizeObserver(([entry]) => setWidth(Math.floor(entry.contentRect.width)))
    observer.observe(ref.current)
    return () => observer.disconnect()
  }, [])

  return { ref, width }
}
