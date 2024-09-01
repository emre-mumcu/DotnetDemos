```cs

// To use routes mapped to controllers as attribute
// MapControllers doesn't make any assumptions about routing and will rely on the user doing attribute routing
app.MapControllers();

// Uses conventional routing (most often used in an MVC application), and sets up the URL route pattern.
// It shorthands the configuration of the default pattern:
// app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();

// Retrun async content
public async Task<IActionResult> Index()
{
	return await Task.Run(() => View());
}

public async Task<IActionResult> Index() => await Task.Run(() => View());
```


# Form with multiple action buttons

https://www.binaryintellect.net/articles/2678a2f2-3236-45a6-a0e5-e6340d9930d5.aspx

## Alternative 1: Multiple buttons with different names

The save and cancel string parameters hold the value of the respective buttons. These parameter names must match the names of the submit buttons on the form. If a button is clicked, its value is received in the action. Otherwise its value will be null.

```html
<form asp-controller="Home" asp-action="ProcessForm" method="post">    
    <input type="submit" name="save" value="Save" />
    <input type="submit" name="cancel" value="Cancel" />
</form>
```
```cs
public IActionResult ProcessForm(string save, string cancel) { }
```

## Alternative 2: Multiple buttons with the same name

If you click the Save button, submit parameter will be Save. If you click on Cancel button the submit parameter will have value of Cancel.

```html
<form asp-controller="Home" asp-action="ProcessForm" method="post">    
    <input type="submit" name="submit" value="Save" />
    <input type="submit" name="submit" value="Cancel" />
</form>
```
```cs
public ActionResult ProcessForm(string submit) { }
```

## Alternative 3: HTML5 formaction and formmethod attributes

```html
<form action="" method="post">
    <input type="submit" value="Option 1" formaction="/Home/SaveForm" />
    <input type="submit" value="Option 2" formaction="/Home/CancelForm" />
</form>

<form>
    <button asp-action="Login" asp-controller="Account" asp-action="login">log in</button>
    <button asp-action="Register" asp-controller="Account" asp-action="signup">sign up</button>
</form>

<form method="post" asp-controller="Account">
	<input type="submit" value="Login" asp-action="Login" />
	<input type="submit" value="Signup" asp-action="Signup" />
</form>
```

## Alternative 4: jQuery / JavaScript code

```html
<form method="post">
    <input type="submit" id="save" value="Save" />
    <input type="submit" id="cancel" value="Cancel" />
</form>
```

```js
$(document).ready(function () {
    $("#save").click(function () {
        $("form").attr("action", "/Home/SaveForm");
    });
    $("#cancel").click(function () {
        $("form").attr("action", "/Home/CancelForm");
    });
});
```

