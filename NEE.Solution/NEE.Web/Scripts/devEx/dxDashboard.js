window.hbDashboard = window.hbDashboard || {};

hbDashboard.colorSchemeIcon = '<svg id="colorSchemeIcon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><defs><style>.dx_gray{fill:#7b7b7b;}</style></defs><title>Themes copy</title><path class="dx_gray" d="M12,3a9,9,0,0,0,0,18c7,0,1.35-3.13,3-5,1.4-1.59,6,4,6-4A9,9,0,0,0,12,3ZM5,10a2,2,0,1,1,2,2A2,2,0,0,1,5,10Zm3,7a2,2,0,1,1,2-2A2,2,0,0,1,8,17Zm3-8a2,2,0,1,1,2-2A2,2,0,0,1,11,9Zm5,1a2,2,0,1,1,2-2A2,2,0,0,1,16,10Z" /></svg>';
//hbDashboard.refreshIcon = '<svg id="refreshIcon" xmlns = "http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 18 18"><defs><style>.dx_gray{fill:#7b7b7b;}</style></defs><path class="dx_gray" d="M9 13.5c-2.49 0-4.5-2.01-4.5-4.5S6.51 4.5 9 4.5c1.24 0 2.36.52 3.17 1.33L10 8h5V3l-1.76 1.76C12.15 3.68 10.66 3 9 3 5.69 3 3.01 5.69 3.01 9S5.69 15 9 15c2.97 0 5.43-2.16 5.9-5h-1.52c-.46 2-2.24 3.5-4.38 3.5z" /></svg>';
hbDashboard.refreshIcon = '<svg id = "refreshIcon" xmlns = "http://www.w3.org/2000/svg" viewBox="0 0 18 18"><path d="M9 13.5c-2.49 0-4.5-2.01-4.5-4.5S6.51 4.5 9 4.5c1.24 0 2.36.52 3.17 1.33L10 8h5V3l-1.76 1.76C12.15 3.68 10.66 3 9 3 5.69 3 3.01 5.69 3.01 9S5.69 15 9 15c2.97 0 5.43-2.16 5.9-5h-1.52c-.46 2-2.24 3.5-4.38 3.5z" /></svg>';

hbDashboard.State = {
    isMobileView: false,
    isDesignerMode: false,
    getColorSchema: function () {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (pair[0] === "colorSchema") { return pair[1]; }
        }
        return "light";
    }
};

function onDashboardTitleToolbarUpdated(args) {
    var colorSchemaList = {
        "light": "Φωτεινό",
        "dark": "Σκοτεινό"
    };

    if (hbDashboard.Sidebar && DevExpress.devices.real().phone) {
        args.options.actionItems.unshift(hbDashboard.Sidebar.getToolbarItem(args.component));
    }

    args.options.actionItems.unshift({
        type: "menu",
        icon: "colorSchemeIcon",
        hint: "Χρώμα",
        menu: {
            items: Object.keys(colorSchemaList).map(function (key) { return colorSchemaList[key] }),
            type: 'list',
            selectionMode: 'single',
            selectedItems: [colorSchemaList[hbDashboard.State.getColorSchema()]],
            itemClick: function (data, element, index) {
                var newTheme = Object.keys(colorSchemaList)[index];
                hbDashboard.Navigation.saveToUrl("colorSchema", newTheme);
                location.reload();
            }
        }
    });

    args.options.actionItems.unshift({
        type: "button",
        icon: "refreshIcon",
        hint: "Ανανέωση Dashboard",
        click: function (element) {
            var dashboardControl = getDashboardControl();
            dashboardControl.refresh();
        }
    });
}

hbDashboard.UI = {
    setButtonCaption: function () {
        var buttonTextElement = document.getElementById("designer-mode-button-text");
        //var modeTitle = document.getElementById("working-mode-title");
        var text = "", title = "";
        if (!buttonTextElement) {
            return;
        }
        if (hbDashboard.State.isMobileView) {
            return;
        }
        var dashboardControl = getDashboardControl();
        if (dashboardControl) {
            text = dashboardControl.isDesignMode() ? "Switch to Viewer" : "Edit in Designer";
            title = dashboardControl.isDesignMode() ? "Designer Mode" : "Viewer Mode";
        }
        buttonTextElement.innerText = text;
       // modeTitle.innerText = title;
    }
};


hbDashboard.Navigation = {
    saveToUrl: function (key, value) {
        var uri = location.href;
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        var newParameterValue = value ? key + "=" + encodeURIComponent(value) : "";
        var newUrl;
        if (uri.match(re)) {
            separator = !!newParameterValue ? '$1' : "";
            newUrl = uri.replace(re, separator + newParameterValue + '$2');
        }
        else if (!!newParameterValue) {
            newUrl = uri + separator + newParameterValue;
        }
        if (newUrl) {
            history.replaceState({}, "", newUrl);
        }
    },
    navigate: function (baseLink) {
        window.location = baseLink + window.location.search;
        window.event.preventDefault ? window.event.preventDefault() : (window.event.returnValue = false);
        return false;
    }
};


function onDashboardChanged(args) {
    var dashboardControl = args.component,
        dashboardId = args.dashboardId;
    if (dashboardId === "CustomItemExtensions") {
        !dashboardControl.findExtension("save-as") && dashboardControl.registerExtension(new SaveAsDashboardExtension(dashboardControl));
    } else {
        dashboardControl.unregisterExtension("save-as");
    }

    hbDashboard.Sidebar && hbDashboard.Sidebar.viewModel && hbDashboard.Sidebar.viewModel.feedback && hbDashboard.Sidebar.viewModel.feedback.init(dashboardId);
}


function onBeforeRender(dashboardControl) {
    DevExpress.Dashboard.ResourceManager.registerIcon(hbDashboard.colorSchemeIcon);
    DevExpress.Dashboard.ResourceManager.registerIcon(hbDashboard.refreshIcon);
    hbDashboard.UI.setButtonCaption();
    hbDashboard.Navigation.saveToUrl("mode", dashboardControl.isDesignMode() ? "designer" : "viewer");

    dashboardControl.isDesignMode.subscribe(function (isDesignValue) {
        hbDashboard.Navigation.saveToUrl("mode", isDesignValue ? "designer" : "viewer");
        hbDashboard.UI.setButtonCaption();
        hbDashboard.State.isDesignerMode = isDesignValue;
    });

    //TODO : Fix Thumbnail Location
    var panelExtension = new DevExpress.Dashboard.DashboardPanelExtension(dashboardControl, { dashboardThumbnail: "/gov/Content/DashboardThumbnail/{0}.png" });
    dashboardControl.registerExtension(panelExtension);
    //Change this to true in order to show Edit in Designer button
    panelExtension.allowSwitchToDesigner(false);
 
}


document.addEventListener("DOMContentLoaded", function (event) {
    var designModeButton = document.getElementById("designer-mode-button"),
        hasDemoToolbar = designModeButton;

    hbDashboard.State.isMobileView = document.querySelector(".phone-wrapper") !== null;

    if (hasDemoToolbar) {
        designModeButton.addEventListener("click", function () {
            var dashboardControl = getDashboardControl();
            if (dashboardControl.isDesignMode()) {
                dashboardControl.switchToViewer();
            } else {
                dashboardControl.switchToDesigner();
            }
        });

        //document.getElementById("info-button").addEventListener("click", function () {
        //    var dashboardControl = getDashboardControl();
        //    hbDashboard.Sidebar.showDemoPopup(dashboardControl, dashboardControl.dashboard());
        //});
    }

    hbDashboard.Sidebar && hbDashboard.Sidebar.initPopup();

    hbDashboard.Feedback && hbDashboard.Feedback.init("https://services.devexpress.com/customerfeedback", window.hbDashboardName || "WebDashboardDemo");

    if (hbDashboard.State.isMobileView) {
        var className = "dx-state-selected";
        document.getElementById("desktop-button").classList.remove(className);
        document.getElementById("mobile-button").classList.add(className);
        designModeButton.classList.add("dx-state-disabled");
    }
});