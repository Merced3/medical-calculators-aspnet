# Decision Log - LACE Calculator

---

## Why Razor Pages?

Simpler for small form-based apps. Easier to maintain and deploy. MVC would be overkill.

---

## Why .NET Core?

Cross-platform, fast startup, deployable to Azure.

---

## Why manual scoring logic?

LACE is simple. No need for database or ML inference yet.

---

## What files were needed to reach MVP?

Just `Index.cshtml` and `Index.cshtml.cs`. It was surprising how fast it came together — I expected way more setup. Razor Pages handles the form-to-code binding cleanly with `[BindProperty]`.

## How does it actually work?

- The Razor form binds input fields to C# properties using `asp-for`.
- On submit, Razor calls `OnPost()` in the PageModel.
- The method calculates the total LACE score and returns it to the same page.
- Because the form posts to itself, everything stays in one view and one class.

---
