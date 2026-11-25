/** Single source of truth for institution and unit branding shown in the UI. */
export const BRAND = {
  university: 'NSBM Green University',
  unit: 'Industry Collaboration Unit',
  unitShort: 'ICU',
  faculty: 'Faculty of Computing',
  tagline: 'Bridging academia and industry',
  unitHead: {
    name: 'Ms. Pavithra Subhashini',
    title: 'Head, Industry Collaboration Unit',
  },
  logo: {
    color: '/brand/nsbm-logo.png',
    white: '/brand/nsbm-logo-white.png',
  },
  website: 'https://www.nsbm.ac.lk',
} as const

export const COPYRIGHT = `© ${new Date().getFullYear()} ${BRAND.university} · ${BRAND.unit}`
