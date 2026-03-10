# MuniEntry Blazor — User Perspective Analysis (Staff Voice, Round 2)
### *A Ground-Level Account from a Court Administrative Staff Member Who's Been on the Python App for Years*

> **See also:** [Pseudo_User_Analysis.md](Pseudo_User_Analysis.md) — original analysis | [Pseudo_User_Response.md](Pseudo_User_Response.md) — the engineering improvement plan | [Legacy_Save_Paths_And_Batch_FTA.md](Legacy_Save_Paths_And_Batch_FTA.md) — legacy save-path behavior and batch FTA mechanics

---

## First Impressions — Day 1

The first thing I noticed was that there was nothing to install. With the Python app I always dreaded the weeks after IT pushed an update because something inevitably broke — a dependency, a path, the wrong Python version — and I'd have to put in a ticket and wait. Opening a browser, signing in with the same Microsoft account I use for Outlook, and just *being in the app* was genuinely surprising.

The forms looked cleaner too, I'll give it that. No more feeling like I was using something from 2008.

But within about twenty minutes, I started looking for things that weren't there.

The Python app asked me which judge's docket I was working the moment it launched. That one question shaped everything that followed — which daily list loaded, which template header got stamped on the DOCX, which subfolder under `M:\Entries\` the file saved to. In Blazor I couldn't find that prompt anywhere. I just started filling out a Fine Only Plea for a Hemmeter case and submitted it without realizing the judge field had defaulted to something else. That DOCX went out wrong. Day one.

Then the DOCX downloaded to my `Downloads` folder with a name like `download (1).docx`. In the Python app the file was named `21CRB1234_Fine_Only_Plea.docx` and was already sitting in `M:\Entries\CrimTraffic\` waiting to be printed. Now I was hunting through Downloads, renaming it, dragging it across to the network share, opening it in Word to verify it looked right. That's four extra steps, forty to sixty times a day.

I also spent a good fifteen minutes looking for the batch FTA button before asking someone.

The overall feeling at end of day one: *it looks nicer but it's slower to actually do my job.*

---

## Week 1 Reflections

By the end of the first week I had a clearer picture of what was structurally different versus what was just unfamiliar.

The EntraID login was already invisible — I stopped noticing it immediately, which is actually a win. And the form validation was noticeably catching things the Python app let slip through. A missing required field used to just render as a blank gap in the printed DOCX and I wouldn't catch it until I was standing at the printer. The Blazor version stopped me before I got there. That was a genuine improvement.

But the DOCX workflow was still causing friction. Every file downloads with an inconsistent name, goes to a personal folder, and the app immediately forgets it ever generated it. If a clerk asked me "did you already do the FTA for 24CRB1234?" there was no way to check. The Python app left timestamped files on `M:\` that served as a de facto log. That was gone entirely and I hadn't realized how much I depended on it.

I also discovered the scheduling entry form issue the hard way during a heavy arraignment morning. The Python `SchedulingEntryDialog` knew which judge I was working for and auto-filled the courtroom. The Blazor version made me pick the judge on every single form submission. With thirty-plus cases that morning it was death by a thousand cuts.

---

## Week 3 Reflections

Three weeks in, I had adapted my muscle memory somewhat. I bookmarked every form I used daily. URL structure was consistent enough that navigation felt reliable.

What I had not adapted to was the lack of session context. I started keeping two browser tabs open — one for Fowler's docket and one for Hemmeter's — and on two separate occasions I submitted through the wrong tab. The app has no awareness of which judge you're working for in a given tab. It doesn't warn you. It just POSTs.

I also discovered there's no confirmation step before generating. In the Python app a modal asked "Generate and save this entry?" before anything happened. In Blazor, clicking Submit fires immediately. I had one accidental submission when I fat-fingered the button while scrolling. The DOCX generated with half-completed data and I had no way to pull it back or even see that it had happened.

The worst thing I discovered at week three: if the browser refreshes mid-form — or my VPN hiccups, which happens — everything I typed is gone. People were copying case numbers into Notepad before filling long forms as a backup. We had invented a shadow process to compensate for something the app should have handled.

---

## Week 6 Reflections

Six weeks in I had a more settled, honest assessment.

The things that were genuinely better: EntraID provisioning was faster for new staff than anything IT had to do with the Python app. The civil and criminal forms being unified in one place instead of two separate Python windows was a quality-of-life improvement I had underestimated. Form validation before printing had measurably reduced reprints.

The things that had hardened into real problems: the office had developed inconsistent filing practices because the app doesn't enforce a save location. Some of us set our browser to auto-download to `M:\Entries\CrimTraffic\`. Some saved to personal folders. One person was just printing directly from Downloads and never filing to the network share at all. We had no audit trail, no enforcement, and no way for a supervisor to pull a report of entries generated that week.

That last point became visible when a filing dispute came up and the answer to "can you show me a log of all entries generated on Tuesday?" was simply: no.

---

## Things I Wish I Had Known

These are the things I would tell myself on day one if I could:

**Set your browser's default download folder to `M:\Entries\CrimTraffic\` before you touch the first form.** The app doesn't save to the network share for you the way the Python app did. If you don't configure this, you will manually move files every single day.

**The judge context does not carry between browser tabs.** If you work two dockets in parallel, pick one tab, set the judge, and stay there. Don't assume the app knows which judge you're working for — it doesn't, and it won't warn you if the field is wrong.

**There is no "your last form is still open" safety net.** In the Python app the dialog stayed on screen after generation. In Blazor the form resets on submit. If you need to correct something, you are starting over. Get in the habit of reviewing before you click Submit, not after.

**"Did it work?" has no answer in the app itself.** Unless a confirmation toast appears, your only proof a DOCX was generated is the file in your Downloads folder. Keep an eye on Downloads after every submission.

**Batch FTA returns a ZIP download now, not an open folder on `M:\Entries\Batch\`.** The old workflow was: click batch FTA, watch files appear in the Batch folder, print from there. The new workflow is: click, get a ZIP, unzip somewhere, print. It's actually faster, but if you go looking for a folder that opened automatically, you'll spend ten minutes confused.

---

## Summary

The honest assessment at six weeks: the foundation is clearly better. No installs, real authentication, cleaner validation, unified forms. But the daily workflow for generating and filing a DOCX is meaningfully slower than what came before, and several gaps — judge context, save path enforcement, an audit log — will keep causing problems until they are addressed.

The new system knew what the old system produced. What it underestimated was how much invisible workflow the old system was silently doing *around* the DOCX that staff never had to think about until it was gone.

---

*Last updated: March 10, 2026*
