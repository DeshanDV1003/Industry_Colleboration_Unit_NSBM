// Builds ../NSBM-ICU-System-Guide.docx from content.cjs and the screenshots in ../screenshots.
// Regenerate after capturing new screenshots:  (cd web && node scripts/capture-docs-screenshots.mjs)  then  npm run build  here.

const fs = require('fs')
const path = require('path')
const {
  AlignmentType, BorderStyle, Document, Footer, Header, HeadingLevel, ImageRun, LevelFormat, Packer, PageBreak,
  PageNumber, Paragraph, ShadingType, Table, TableCell, TableOfContents, TableRow, TextRun, WidthType,
} = require('docx')
const content = require('./content.cjs')

const SHOTS = path.resolve(__dirname, '../screenshots')
const OUT = path.resolve(__dirname, '../NSBM-ICU-System-Guide.docx')

// A4, 1-inch margins: 11906 x 16838 DXA; text width 9026 DXA (6.27 in).
const PAGE = { width: 11906, height: 16838, margin: 1440 }
const TEXT_WIDTH = PAGE.width - 2 * PAGE.margin
const BLUE = '0B5CAD'
const GREEN = '0B7A43'
const INK = '1F2937'
const MUTED = '5B6B7F'
const FONT = 'Arial'

// ---------------------------------------------------------------- small builders
const text = (t, opts = {}) => new TextRun({ text: t, font: FONT, ...opts })
const p = (children, opts = {}) => new Paragraph({ children: Array.isArray(children) ? children : [text(children)], spacing: { after: 120 }, ...opts })
const bullet = (t, level = 0) => new Paragraph({ children: [text(t)], numbering: { reference: 'bullets', level }, spacing: { after: 60 } })
const numbered = (t, ref) => new Paragraph({ children: [text(t)], numbering: { reference: ref, level: 0 }, spacing: { after: 60 } })
const label = (t) => new Paragraph({ children: [text(t, { bold: true, color: BLUE, size: 21 })], spacing: { before: 120, after: 60 }, keepNext: true })
const h1 = (t) => new Paragraph({ heading: HeadingLevel.HEADING_1, children: [text(t)], pageBreakBefore: true })
const h2 = (t) => new Paragraph({ heading: HeadingLevel.HEADING_2, children: [text(t)] })
const h3 = (t) => new Paragraph({ heading: HeadingLevel.HEADING_3, children: [text(t)] })

const border = { style: BorderStyle.SINGLE, size: 4, color: 'D5DCE6' }
const borders = { top: border, bottom: border, left: border, right: border }

/** A table with fixed DXA widths on the table and every cell (required for Word and Google Docs). */
function table(header, rows, widths) {
  const cell = (value, isHeader, width) => new TableCell({
    borders,
    width: { size: width, type: WidthType.DXA },
    shading: isHeader ? { fill: 'E8F0FA', type: ShadingType.CLEAR, color: 'auto' } : undefined,
    margins: { top: 80, bottom: 80, left: 110, right: 110 },
    children: [new Paragraph({ children: [text(String(value), { bold: isHeader, size: 19 })] })],
  })
  return new Table({
    width: { size: widths.reduce((a, b) => a + b, 0), type: WidthType.DXA },
    columnWidths: widths,
    rows: [
      new TableRow({ tableHeader: true, children: header.map((h, i) => cell(h, true, widths[i])) }),
      ...rows.map((r) => new TableRow({ children: r.map((v, i) => cell(v, false, widths[i])) })),
    ],
  })
}

/** PNG width and height from the IHDR chunk. */
function pngSize(file) {
  const b = fs.readFileSync(file)
  return { width: b.readUInt32BE(16), height: b.readUInt32BE(20) }
}

let figureNo = 0
const figures = []

/** A screenshot scaled to the text width (or to fit the page height), framed, with a numbered caption. */
function figure(shot, caption) {
  const file = path.join(SHOTS, shot)
  if (!fs.existsSync(file)) throw new Error(`Missing screenshot ${shot}`)
  const { width, height } = pngSize(file)
  const maxW = 602 // px at 96 dpi = 6.27 in
  const maxH = 690 // leaves room for a heading above, so the first section can start right under a chapter's intro
  const scale = Math.min(maxW / width, maxH / height)
  figureNo += 1
  figures.push({ no: figureNo, caption })
  return [
    new Paragraph({
      alignment: AlignmentType.CENTER,
      keepNext: true,
      spacing: { before: 120, after: 60 },
      border: { top: border, bottom: border, left: border, right: border },
      children: [new ImageRun({ type: 'png', data: fs.readFileSync(file), transformation: { width: Math.round(width * scale), height: Math.round(height * scale) },
        altText: { title: caption, description: caption, name: path.basename(file) } })],
    }),
    new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 200 }, children: [text(`Figure ${figureNo}. ${caption}`, { italics: true, size: 18, color: MUTED })] }),
  ]
}

// ---------------------------------------------------------------- document parts
function cover() {
  const m = content.meta
  return [
    new Paragraph({ spacing: { before: 2600 }, children: [text(m.organisation.toUpperCase(), { bold: true, color: GREEN, size: 22 })] }),
    new Paragraph({ spacing: { before: 200 }, children: [text(m.title, { bold: true, size: 56, color: INK })] }),
    new Paragraph({ spacing: { before: 120, after: 600 }, children: [text(m.subtitle, { size: 32, color: BLUE })],
      border: { bottom: { style: BorderStyle.SINGLE, size: 12, color: GREEN, space: 12 } } }),
    p([text('Prepared for: ', { bold: true }), text(m.preparedFor)]),
    p([text('Unit head: ', { bold: true }), text(m.unitHead)]),
    p([text('Version: ', { bold: true }), text(`${m.version}, ${m.date}`)]),
    new Paragraph({ spacing: { before: 2400 }, children: [text('Covers the public pages and all five portals: Student, Lecturer, Industry partner, Assistant Registrar and Unit admin.', { color: MUTED, size: 20 })] }),
    new Paragraph({ children: [new PageBreak()] }),
  ]
}

function documentControl() {
  const m = content.meta
  return [
    new Paragraph({ heading: HeadingLevel.HEADING_1, children: [text('Document control')] }),
    table(['Item', 'Detail'], [
      ['Document', `${m.title}: ${m.subtitle}`],
      ['Version', m.version],
      ['Date', m.date],
      ['Prepared for', m.preparedFor],
      ['System covered', 'Public pages; Student, Lecturer, Industry partner, Assistant Registrar and Unit admin portals'],
      ['Screenshots', 'Captured from the running system with fictional demonstration data'],
    ], [2400, TEXT_WIDTH - 2400]),
    new Paragraph({ spacing: { before: 400 }, children: [] }),
    new Paragraph({ heading: HeadingLevel.HEADING_1, children: [text('Contents')], pageBreakBefore: true }),
    new TableOfContents('Contents', { hyperlink: true, headingStyleRange: '1-2' }),
  ]
}

function introduction() {
  const i = content.introduction
  return [
    // The contents list already ends a page, so the first chapter does not force another page break.
    new Paragraph({ heading: HeadingLevel.HEADING_1, children: [text('1. Introduction')] }),
    h2('1.1 Purpose'), ...i.purpose.map((t) => p(t)),
    h2('1.2 Audience'), ...i.audience.map((t) => bullet(t)),
    h2('1.3 How to read this guide'), ...i.howToRead.map((t) => p(t)),
    p([text('About the screenshots. ', { bold: true }), text(i.dataNote)]),
  ]
}

function overview() {
  const o = content.overview
  return [
    h1('2. The platform at a glance'),
    h2('2.1 User groups and portals'),
    p('Each user group has its own portal, opened automatically after sign-in. Its menu shows only what that role may do.'),
    table(['Portal', 'Who uses it', 'Main features'], o.portals, [1800, 2300, TEXT_WIDTH - 4100]),
    h2('2.2 Features every portal shares'),
    table(['Feature', 'What it does'], o.shared, [2300, TEXT_WIDTH - 2300]),
    h2('2.3 Responsible-use principles'),
    p('The platform is built around these rules, which appear throughout this guide as notes on the pages they affect:'),
    ...o.principles.map((t) => bullet(t)),
  ]
}

/** One chapter per portal: intro, then each page as a numbered section with figure, purpose, actions and notes. */
function chapter(ch, number, pageIndex) {
  const out = [h1(`${number}. ${ch.title}`), p(ch.intro)]
  let n = 0
  for (const s of ch.sections) {
    if (s.group) { out.push(new Paragraph({ children: [text(s.group.toUpperCase(), { bold: true, color: GREEN, size: 20 })], spacing: { before: 280, after: 60 }, keepNext: true })); continue }
    n += 1
    const id = `${number}.${n}`
    out.push(h2(`${id} ${s.title}`))
    out.push(new Paragraph({ keepNext: true, spacing: { after: 80 }, children: [text('Address: ', { bold: true, size: 18, color: MUTED }), text(s.route, { size: 18, color: MUTED, font: 'Consolas' })] }))
    out.push(...figure(s.shot, `${ch.title} - ${s.title}`))
    out.push(label('What it is for'), p(s.purpose))
    if (s.actions?.length) { out.push(label('What you can do')); s.actions.forEach((a) => out.push(bullet(a))) }
    if (s.notes?.length) { out.push(label('Rules and notes')); s.notes.forEach((a) => out.push(bullet(a))) }
    pageIndex.push([ch.title.replace(/ portal$| pages$/, ''), s.title, s.route, id])
  }
  return out
}

function workflows(number) {
  const out = [h1(`${number}. Workflows across portals`), p('The main processes involve more than one portal. Each is listed step by step, with the portal where each step happens described in the sections above.')]
  content.workflows.forEach((w, i) => {
    out.push(h2(`${number}.${i + 1} ${w.title}`))
    w.steps.forEach((step) => out.push(numbered(step, `wf${i}`)))
  })
  return out
}

function appendices(pageIndex) {
  return [
    h1('Appendix A. Page index'),
    p('Every page and dialog in this guide, with its address in the system and the section that describes it.'),
    table(['Portal', 'Page', 'Address', 'Section'], pageIndex, [1700, 3326, 2800, 1200]),

    h1('Appendix B. Demo mode'),
    p('For demonstrations the platform can run in demo mode with fictional data. It is switched off in a real deployment; the production build of the web app does not contain these controls at all.'),
    bullet('The sign-in page shows "Explore a demo portal" buttons that open each portal with a fictional account (visible in Figure 2).'),
    bullet('The sidebar shows a demo role switcher to move between portals (below).'),
    bullet('Unit admins can reset all demo data to its original state from Settings. The audit log is never reset.'),
    ...figure('demo/01-role-switcher.png', 'Demo mode - the demo role switcher at the bottom of the sidebar'),

    h1('Appendix C. Glossary'),
    table(['Term', 'Meaning'], content.glossary, [2300, TEXT_WIDTH - 2300]),
  ]
}

// ---------------------------------------------------------------- assemble
function build() {
  const pageIndex = []
  const body = [...documentControl(), ...introduction(), ...overview()]
  content.chapters.forEach((ch, i) => body.push(...chapter(ch, i + 3, pageIndex)))
  body.push(...workflows(content.chapters.length + 3))
  body.push(...appendices(pageIndex))

  const numberingConfig = [
    { reference: 'bullets', levels: [{ level: 0, format: LevelFormat.BULLET, text: '•', alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 540, hanging: 270 } } } },
      { level: 1, format: LevelFormat.BULLET, text: '◦', alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 1080, hanging: 270 } } } }] },
    ...content.workflows.map((_, i) => ({ reference: `wf${i}`, levels: [{ level: 0, format: LevelFormat.DECIMAL, text: '%1.', alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 540, hanging: 300 } } } }] })),
  ]

  const doc = new Document({
    creator: 'Deshan Vimukthi',
    title: `${content.meta.title}: ${content.meta.subtitle}`,
    description: 'System guide for NSBM Green University and the Industry Collaboration Unit',
    styles: {
      default: { document: { run: { font: FONT, size: 21, color: INK } } },
      paragraphStyles: [
        { id: 'Heading1', name: 'Heading 1', basedOn: 'Normal', next: 'Normal', quickFormat: true,
          run: { font: FONT, size: 34, bold: true, color: BLUE }, paragraph: { spacing: { before: 0, after: 200 }, keepNext: true, outlineLevel: 0 } },
        { id: 'Heading2', name: 'Heading 2', basedOn: 'Normal', next: 'Normal', quickFormat: true,
          run: { font: FONT, size: 26, bold: true, color: INK }, paragraph: { spacing: { before: 320, after: 100 }, keepNext: true, outlineLevel: 1 } },
        { id: 'Heading3', name: 'Heading 3', basedOn: 'Normal', next: 'Normal', quickFormat: true,
          run: { font: FONT, size: 22, bold: true, color: INK }, paragraph: { spacing: { before: 200, after: 80 }, keepNext: true, outlineLevel: 2 } },
      ],
    },
    numbering: { config: numberingConfig },
    features: { updateFields: true },
    sections: [
      { properties: { page: { size: { width: PAGE.width, height: PAGE.height }, margin: { top: PAGE.margin, bottom: PAGE.margin, left: PAGE.margin, right: PAGE.margin } } },
        children: cover() },
      {
        properties: { page: { size: { width: PAGE.width, height: PAGE.height }, margin: { top: PAGE.margin, bottom: PAGE.margin, left: PAGE.margin, right: PAGE.margin } } },
        headers: { default: new Header({ children: [new Paragraph({ alignment: AlignmentType.RIGHT, children: [text('NSBM Industry Collaboration Unit Platform · System guide', { size: 16, color: MUTED })] })] }) },
        footers: { default: new Footer({ children: [new Paragraph({ alignment: AlignmentType.CENTER, children: [text('Page ', { size: 16, color: MUTED }), new TextRun({ children: [PageNumber.CURRENT], font: FONT, size: 16, color: MUTED })] })] }) },
        children: body,
      },
    ],
  })

  return Packer.toBuffer(doc).then((buffer) => {
    fs.writeFileSync(OUT, buffer)
    console.log(`Wrote ${OUT}: ${figureNo} figures, ${pageIndex.length} pages described.`)
  })
}

build().catch((e) => { console.error(e); process.exit(1) })
