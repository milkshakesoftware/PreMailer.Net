# Research: Expanding PreMailer.Net into the Pre-Send Space

*Research document, July 2026. Everything that happens to an email before it is sent is our turf; post-send signals matter insofar as they feed the next send.*

---

## 1. Where PreMailer.Net sits today

PreMailer.Net is the de-facto CSS inliner for .NET — it appears as the default inlining step in virtually every Razor-to-email walkthrough and is bundled into wrapper libraries like RG.RazorMail. The typical pipeline it lives in is:

```
Razor / RazorLight / Blazor HtmlRenderer   (templating)
        → PreMailer.Net                    (inlining)
        → FluentEmail                      (composition)
        → MailKit / SendGrid / Mailgun     (delivery)
```

Today the library does one transformation (CSS → inline styles) plus a few extras: UTM tagging (`AddAnalyticsTags`), `-premailer-*` CSS-to-attribute proxying, an email-safe formatter (opt-in), and raw DOM access via `Document`.

**The niche is uncontested but commoditized.** No serious .NET inliner alternative exists, but Mjml.Net ships its own inlining post-processor, so component-framework users can bypass us entirely. Meanwhile, everything the wider ecosystem built *around* inlining has **no .NET equivalent at all**. That surrounding space is the opportunity.

## 2. The strategic frame

Three findings from the market research define the opening:

1. **API-first pre-send QA is underserved and getting worse.** Litmus now starts at $500/month (entry plan eliminated, Aug 2025). Email on Acid is being folded into Mailgun Inspect from mid-2026. Testi.at and Parcel have no public API. Only Mailtrap and mail-tester offer programmatic checks — none in .NET, none as a library.
2. **The building blocks are free.** The caniemail client-support dataset is MIT-licensed with a stable JSON API and has JS consumers (doiuse-email, Parcel, Mailtrap) but **no .NET consumer**. Postmark's SpamCheck API (SpamAssassin) is free and keyless. The Email Markup Consortium publishes a concrete, statically-checkable accessibility rule spec.
3. **Almost the entire industry pre-send checklist is statically analyzable from a DOM we already hold.** Link QA, alt text, preheader detection, merge-tag leaks, unsubscribe presence, Gmail's 102KB clipping threshold, image rules, dark-mode pitfalls, client-compat scoring — all computable from the AngleSharp document PreMailer.Net has already parsed. The marginal cost of analysis is near zero; the parse is paid for.

The strategic move is repositioning from *"a CSS inliner"* to *"the pre-flight compiler and linter for email in .NET"* — the last programmable step before send. Maizzle is the proof of shape: its pipeline runs ~20 transformers, of which inlining is just one (purge unused CSS, minify, six-digit hex, fill missing attributes, URL params, MSO helpers, widow words, plaintext…). The whole Maizzle-style transformer catalogue is open roadmap territory in .NET.

## 3. What users are already asking for (demand signals)

From this repo's issue tracker and ecosystem mentions, ranked by weight:

| Demand cluster | Evidence | Notes |
|---|---|---|
| Output fidelity ("don't mangle my HTML") | ~10 issues: entities decoded (#126, #347), URLs re-encoded breaking tracking (#172, #193), VML/`xmlns` stripped killing Outlook hacks (#308, #54), `!important` handling (#140, #410), broken `useEmailFormatter` overload (#430, #434) | Largest bug cluster. Entity expansion also inflates size toward Gmail clipping. |
| Performance, async, bulk | #438 (concurrency deadlock, production-blocking), #233/#152 (regex backtracking hangs), #290, #129 ("inline 1000 emails takes 40s — let me parse the template once") | No async API surface exists today. |
| Modern CSS downleveling | #339 (`var()` resolution — most-reacted open feature), #246 (`hsl()`→`rgb()`), #368 (fallback chains dropped), #82/#106 (pseudo-class handling) | Driven by Bootstrap/CKEditor emitting variable-heavy CSS. juice already ships `resolveCSSVariables`. |
| Fine-grained preservation | #422 (classes referenced by preserved media queries get stripped), #340 (class allowlist), #160, #123 | Three separate issues asking for the same knob. |
| Downloader control | #173 (User-Agent), #157 (tolerate 404s), #235 (data-URIs) | |

Ecosystem-level gaps voiced in blogs/Stack Overflow: no .NET plain-text generation story (Ruby premailer has had it for a decade), hand-rolled glue between Razor/PreMailer/FluentEmail in every tutorial, FluentEmail itself stale, and "MJML envy" with no first-class .NET responsive authoring layer.

## 4. Opportunity map

### 4.1 Foundation hardening (prerequisite, not optional)

Any expansion is built on trust in the core transform. Before adding surface area:

- **True async API** (`MoveCssInlineAsync`) and thread-safety — fixes the #438 class of failures and unlocks server workloads.
- **Template mode: parse once, inline many.** A compiled-template object that amortizes parsing/CSS resolution across a bulk send (#129). This is *the* bulk-email feature and no competitor has it because no competitor is a library in the send path.
- **Email-safe serialization by default** — entity preservation, no URL re-encoding, VML/namespace preservation, conditional-comment safety. Un-break `useEmailFormatter` and make it the default path.
- Regex hardening (`Regex.MatchTimeout`, rewrite of `CssParser.CleanUp`), downloader options (User-Agent, tolerate failures, data-URI handling).

### 4.2 A transformer pipeline (the Maizzle move)

Refactor the monolithic `MoveCssInline` into an ordered pipeline of composable transformers with one options object. Inlining becomes the flagship transformer among peers:

| Transformer | What it does | Prior art |
|---|---|---|
| CSS downleveling | Resolve `var()`, convert `hsl()`/`oklch()`→`rgb`, preserve duplicate-property fallback chains, expand 3-digit hex to 6 | juice, Maizzle `sixHex` — answers #339/#246/#368 |
| Unused CSS purge | After inlining, drop `<style>` rules matching nothing; keep media queries/pseudos/dark-mode blocks; track classes referenced by preserved media queries | email-comb; answers #422 directly |
| Email minifier | Whitespace collapse, comment stripping (preserving `<!--[if mso]-->`), SMTP 998-char line-length safety, quoted-printable-aware size accounting | html-crush |
| Attribute filler | `role="presentation"` on layout tables, `alt=""` on decorative images, `width`/`height` attributes copied from CSS, `border=0` | Maizzle `addAttributes`; the single highest-impact accessibility transform |
| Pseudo/media preservation | Move non-inlinable pseudo styles into a `<style>` block instead of dropping (#82); `preserveFontFaces`/`preserveKeyFrames` | juice |
| Head injection | `lang`/`dir`/`<title>`, dark-mode meta (`color-scheme`, `supported-color-schemes`), MSO DPI fix (`o:PixelsPerInch`, `xmlns:v/o`) | All deterministic, all currently hand-written by every .NET email dev |
| Outlook helpers | Generate VML "bulletproof buttons" from styled `<a>` elements (fully deterministic from computed padding/background/radius), VML background images, `mso-line-height-rule` injection | buttons.cm, MJML |
| URL tools | Absolutize relative URLs against `baseUri` (Ruby premailer parity), generalized query-param appender (superset of today's `AddAnalyticsTags`) | Maizzle `urlParameters` |
| Template-token safety | Preserve `{{ }}`, `*|TAG|*`, `%%var%%`, `<%= %>` through parsing/serialization | python-premailer `preserve_handlebar_syntax`; recurring need everywhere |

### 4.3 `PreMailer.Analyze()` — the pre-flight report (the headline feature)

A lint/audit API over the same DOM, returning a structured, severity-graded report. This is the feature that changes what the library *is*, and every rule below is static — no network, no sending:

- **Size & clipping**: encoded-size estimate vs Gmail's 102,400-byte clip threshold (warn >85KB, fail >102KB; non-ASCII counts multi-byte). Flag when the unsubscribe link or tracking pixel falls below the projected clip point. Especially apt for us since *inlining inflates size* — we cause the problem, we should measure it.
- **Client compatibility scoring** via the caniemail MIT dataset: score used CSS/HTML features against a target client set (`gmail.*`, `outlook.windows`, …), like doiuse-email but for .NET. Ship a snapshot, allow refresh from the JSON API.
- **Accessibility** per the Email Markup Consortium spec (99.89% of production emails fail it): missing `lang`/`dir`/`<title>`, tables without `role="presentation"`, missing/empty alt, WCAG contrast ratios (computable from inlined styles: 4.5:1 normal / 3:1 large text), generic link text, color-only links, sub-14px fonts.
- **Link & content QA**: placeholder hrefs (`#`, empty, `javascript:`, localhost/private IPs), http:// images, unresolved merge tags (regex family per ESP), UTM consistency, visible-unsubscribe presence + physical-address heuristic (CAN-SPAM), preheader presence/length (35–100 chars), tracking-pixel detection.
- **Dark mode lints**: missing color-scheme meta, no `prefers-color-scheme` block, pure `#FFF`/`#000` inversion hazards, transparent-PNG warnings, light-border-on-light-background inversion traps.
- **Image rules**: missing dimensions, base64 data-URIs (Gmail blocks; counts toward clipping), CID caveats, image-to-text ratio heuristics matching SpamAssassin's `HTML_IMAGE_RATIO_*` rules (target ≥60% text).
- **Bulk-sender compliance report** (Gmail/Yahoo 2024 rules, permanent rejections since 2025): everything checkable in-body (visible unsub, no hidden unsub, address, valid HTML), plus an explicit checklist of the header/DNS-side items we *cannot* verify (SPF/DKIM/DMARC, RFC 8058 `List-Unsubscribe-Post`, spam-rate <0.3%) — telling users where our visibility ends is itself a feature.
- Optional network tier: live link checking (HEAD requests) and spam scoring via Postmark's free SpamCheck API.

### 4.4 Companion outputs

- **Plain-text generation** (`ToPlainText()`): the clearest feature-parity gap vs Ruby premailer. HTML-only mail costs SpamAssassin points (`MIME_HTML_ONLY`); watch surfaces and screen readers consume the text part. Rules are well-established (links as `Text <URL>`, headings uppercased, lists prefixed, skip `display:none` except the unsubscribe footer, ~78-char wrap). We hold the parsed DOM; this is cheap.
- **AMP-for-email / dark-mode variant emission** — lower priority, but the pipeline architecture makes variants natural.

### 4.5 Ecosystem & distribution

Every blog post reinvents the same glue. Own it:

- `PreMailer.Net.Extensions.DependencyInjection` (`services.AddPreMailer()`), a FluentEmail decorator, Razor/Blazor `HtmlRenderer` integration.
- **A `dotnet premailer` CLI tool + GitHub Action**: inline + analyze in CI, fail the build on lint errors — this is how the Analyze() report becomes a team workflow rather than a method call, and it's the cheapest possible "service" to ship (zero hosting).
- MSBuild task for compile-time template processing.

### 4.6 Post-send, tied to the next send

Per the strategic frame, post-send is in scope only as feedback into the next email:

- **Client-mix-weighted compatibility scoring.** The generic caniemail score treats all clients equally; the useful score weights by *your audience's actual client distribution*. Let users feed their open-analytics client mix (Litmus/ESP exports, or user-agent data their tracking already collects) into `Analyze(targets)` so the next send is linted against the clients that actually matter to them. This is the cleanest post-send→pre-send loop and nobody offers it as a library.
- **UTM round-trip.** `AddAnalyticsTags` already stamps the outbound links; extend with campaign-naming conventions and a helper for correlating GA/ESP results back to template versions (e.g. a `utm_content` scheme keyed to template + variant), making A/B iteration on templates systematic.
- **Engagement-driven lint priorities** (longer term): if spam-rate or clipping incidents rise, the next `Analyze()` should say so — plausibly via a hosted service (below) that remembers report history per template.

## 5. Services / commercial layer

The library stays MIT; value-added services sit above it. Ordered by effort:

1. **GitHub Action + CLI** (free, distribution play) — establishes "pre-flight in CI" as a habit with our name on it.
2. **Hosted pre-flight API** — POST HTML, get the full Analyze() report + spam score + live link check as JSON. Mailtrap-style developer pricing (freemium, $10–50/mo) undercuts Litmus's $500/mo floor by two orders of magnitude for the automation use case. The lint/score/compat portion is pure CPU — no seed inboxes or device farm needed.
3. **Report history & badge** — per-template score tracking over time ("email health" dashboard), shareable badge, the natural home for the post-send feedback loop in 4.6.
4. **Screenshot previews** — explicitly *not* recommended: requires a device/client farm, is Litmus/Mailgun-Inspect's moat, and the Litmus Instant API can be integrated instead if users want it.

## 6. Prioritized roadmap

| Phase | Items | Rationale |
|---|---|---|
| **1. Trust** | Async API, template/bulk mode, email-safe serializer by default, regex + downloader hardening | Directly answers the heaviest issue clusters (#438, #129, #233, fidelity bugs); prerequisite for everything else |
| **2. Pipeline** | Transformer architecture; CSS downleveling (`var()`, `hsl()`, fallbacks); unused-CSS purge with media-query awareness; attribute filler; head injection | Answers #339/#246/#368/#422; Maizzle-proven shapes; each transformer is independently shippable |
| **3. Analyze()** | Size/clipping report, caniemail scoring, accessibility lints, link/content QA, compliance report | The repositioning feature; zero-cost data (MIT/free APIs); no .NET competitor |
| **4. Outputs & glue** | `ToPlainText()`, VML button generation, minifier, DI/FluentEmail/Razor packages, CLI + GitHub Action | Parity with Ruby premailer + distribution |
| **5. Services** | Hosted pre-flight API, report history, client-mix-weighted scoring | Monetization once 1–4 have grown the funnel |

## 7. Risks

- **Maintenance capacity** is the binding constraint (#350 "looking for maintainers?", 23 comments; stale-issue bulk closures). Phases 1–2 reduce issue load; phases 3+ add surface area. The transformer architecture mitigates this by making features independently testable and community-contributable.
- **AngleSharp coupling**: serialization-fidelity bugs originate downstream; the email-safe formatter is our containment strategy and must be owned seriously.
- **Mjml.Net** could grow the same checks; our moat is being framework-agnostic (any HTML in, not just MJML) and sitting in the existing Razor-heavy .NET install base.
- **caniemail data drift**: ship snapshots with the package, version them, and allow runtime refresh — never hard-depend on the live endpoint.

## Appendix: source highlights

- Market: Litmus pricing/API (docs.litmus.com/instant), Email on Acid → Mailgun Inspect transition, Mailtrap HTML Check, Parcel checkers, mail-tester JSON API, GlockApps API 2.0, Postmark SpamCheck (spamcheck.postmarkapp.com/doc), caniemail dataset (github.com/hteumeuleu/caniemail, MIT).
- Ecosystem: Maizzle transformers (maizzle.com/docs/transformers), juice options, python/ruby premailer options, email-comb & html-crush (codsen.com), MJML MSO/VML emission, react-email `render({ plainText })`.
- Technical specs: Gmail 102KB clipping (hteumeuleu/email-bugs#41), Email Markup Consortium accessibility reports (emailmarkup.org), WCAG contrast thresholds, Gmail/Yahoo bulk-sender requirements (support.google.com/mail/answer/14229414), RFC 8058, SpamAssassin image-ratio rules.
- Demand: milkshakesoftware/PreMailer.Net issues #438, #339, #422, #368, #246, #233, #129, #82, plus RG.RazorMail, Scott Sauber / Fusonic / End Point Dev integration posts.
