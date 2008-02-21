function createjsDOMenu() {
  cursorMenu1 = new jsDOMenu(150);
  with (cursorMenu1) {
    addMenuItem(new menuItem("Item 1", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 2", "", "blank.htm"));
    addMenuItem(new menuItem("Item 3", "item3", "blank.htm"));
    addMenuItem(new menuItem("Item 4", "", "blank.htm"));
  }
  
  cursorMenu1_1 = new jsDOMenu(130);
  with (cursorMenu1_1) {
    addMenuItem(new menuItem("Item 1", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 2", "item2", "blank.htm"));
    addMenuItem(new menuItem("Item 3", "", "blank.htm"));
    addMenuItem(new menuItem("Item 4", "item4", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 5", "", "blank.htm"));
  }
  
  cursorMenu1_1_1 = new jsDOMenu(160);
  with (cursorMenu1_1_1) {
    addMenuItem(new menuItem("Item 1", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 2", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 3", "", "blank.htm"));
    addMenuItem(new menuItem("Item 4", "", "blank.htm"));
  }
  
  cursorMenu1_1_2 = new jsDOMenu(140);
  with (cursorMenu1_1_2) {
    addMenuItem(new menuItem("Item 1", "item1", "blank.htm"));
    addMenuItem(new menuItem("Item 2", "", "blank.htm"));
    addMenuItem(new menuItem("Item 3", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 4", "", "blank.htm"));
    addMenuItem(new menuItem("Item 5", "", "blank.htm"));
  }
  
  cursorMenu1_1_2_1 = new jsDOMenu(120);
  with (cursorMenu1_1_2_1) {
    addMenuItem(new menuItem("Item 1", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 2", "", "blank.htm"));
    addMenuItem(new menuItem("Item 3", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 4", "", "blank.htm"));
    addMenuItem(new menuItem("Item 5", "item5", "blank.htm"));
  }
  
  cursorMenu1_1_2_1_1 = new jsDOMenu(130);
  with (cursorMenu1_1_2_1_1) {
    addMenuItem(new menuItem("Item 1", "", "blank.htm"));
    addMenuItem(new menuItem("-"));
    addMenuItem(new menuItem("Item 2", "", "blank.htm"));
    addMenuItem(new menuItem("Item 3", "", "blank.htm"));
    addMenuItem(new menuItem("Item 4", "", "blank.htm"));
  }
  
  cursorMenu1.items.item3.setSubMenu(cursorMenu1_1);
  cursorMenu1_1.items.item2.setSubMenu(cursorMenu1_1_1);
  cursorMenu1_1.items.item4.setSubMenu(cursorMenu1_1_2);
  cursorMenu1_1_2.items.item1.setSubMenu(cursorMenu1_1_2_1);
  cursorMenu1_1_2_1.items.item5.setSubMenu(cursorMenu1_1_2_1_1);
  
  cursorMenu1.items.item3.showIcon("icon1", "icon1");
  cursorMenu1_1.items.item2.showIcon("icon2", "icon2");
  cursorMenu1_1.items.item4.showIcon("icon3", "icon3");
  cursorMenu1_1_2.items.item1.showIcon("icon1", "icon1");
  cursorMenu1_1_2_1.items.item5.showIcon("icon1", "icon1");
  
  setPopUpMenu(cursorMenu1);
  activatePopUpMenuBy(1, 2);
}