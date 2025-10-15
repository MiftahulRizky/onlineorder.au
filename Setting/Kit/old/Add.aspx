<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Add.aspx.vb" Inherits="Setting_Kit_Add" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Add Hardware Kit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Hardware Kit</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Add Hardware Kit</h3>
                        </div>
                        
                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">KIT ID</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtId" CssClass="form-control" placeholder="Id ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">KIT NAME</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DESIGN TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">BLIND TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBlind" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">BRACKET TYPE</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBracketType" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 col-sm-12 col-md-12">
                                    <a href="#" class="btn btn-secondary " data-bs-toggle="modal" data-bs-target="#modalBracket">
                                        <i class="bi bi-plus-circle me-2"></i>Add New
                                    </a>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">TUBE TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlTubeType" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 col-sm-12 col-md-12">
                                    <a href="#" class="btn btn-secondary " data-bs-toggle="modal" data-bs-target="#modalTube">
                                        <i class="bi bi-plus-circle me-2"></i>Add New
                                    </a>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">CONTROL TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlControlType" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 col-sm-12 col-md-12">
                                    <a href="#" class="btn btn-secondary " data-bs-toggle="modal" data-bs-target="#modalControl">
                                        <i class="bi bi-plus-circle me-2"></i>Add New
                                    </a>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">COLOUR TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlColourType" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-lg-2 col-sm-12 col-md-12">
                                    <a href="#" class="btn btn-secondary " data-bs-toggle="modal" data-bs-target="#modalColour">
                                        <i class="bi bi-plus-circle me-2"></i>Add New
                                    </a>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DESCRITPION</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">ACTIVE</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                        <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            
                            <div class="row" runat="server" id="divError">
                                <div class="col-lg-12">
                                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                        <div class="d-flex">
                                            <div>
                                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                            </div>
                                            <div>
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-end">
                            <asp:LinkButton runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnSubmit_Click" >
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i> Cancel
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Information</h3>
                        </div>
                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalBracket" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Bracket Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" ID="txtBracketName" CssClass="form-control " placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Description :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtBracketDescription" Height="100px" CssClass="form-control" ReadOnly="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Active :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlBracketActive" CssClass="form-select ">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn me-auto " data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    <asp:LinkButton runat="server" ID="btnBracketSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnBracketSubmit_Click" >
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalControl" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Control Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" ID="txtControlName" CssClass="form-control " placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Description :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtControlDescription" Height="100px" CssClass="form-control" ReadOnly="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Active :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlControlActive" CssClass="form-select ">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn me-auto " data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    </button>
                    <asp:LinkButton runat="server" ID="btnControlSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnControlSubmit_Click" >
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalTube" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Tube Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" ID="txtTubeName" CssClass="form-control " placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Description :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtTubeDescription" Height="100px" CssClass="form-control" ReadOnly="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Active :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlTubeActive" CssClass="form-select ">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn me-auto " data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    <asp:LinkButton runat="server" ID="btnTubeSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnTubeSubmit_Click" >
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalColour" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Colour Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" ID="txtColourName" CssClass="form-control " placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Description :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtColourDescription" Height="100px" CssClass="form-control" ReadOnly="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Active :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlColourActive" CssClass="form-select ">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn me-auto " data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    <asp:LinkButton runat="server" ID="btnColourSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnColourSubmit_Click" >
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        const id = document.getElementById("MainContent_txtId");
        id.addEventListener("input", (e) => {
            e.target.classList.remove('is-invalid');
        })
        
        const kitName = document.getElementById("MainContent_txtName");
        kitName.addEventListener("input", (e) => {
           e.target.classList.remove('is-invalid');
        })

        // const designName = document.getElementById("MainContent_ddlDesign");
        // designName.addEventListener("change", (e) => {
        //    e.target.classList.remove('is-invalid');
        // })

        const blind = document.getElementById("MainContent_ddlBlind");
        blind.addEventListener("input", (e) => {
           e.target.classList.remove('is-invalid');
        })
        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
            });
        }
    </script>
    
    <div runat="server" visible="false">
        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO HardwareKits VALUES (NEWID(), @SoeId, @DesignId, @BlindId, @Name, @BracketType, @TubeType, @ControlType, @Colour, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="txtId" Name="SoeId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlDesign" Name="DesignId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlBlind" Name="BlindId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBracketType" Name="BracketType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlTubeType" Name="TubeType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlControlType" Name="ControlType" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlColourType" Name="Colour" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsBracket" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO BracketType VALUES (NEWID(), @Name, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="txtBracketName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBracketDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBracketActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsControl" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO ControlType VALUES (NEWID(), @Name, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="txtControlName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtControlDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlControlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsTube" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO TubeType VALUES (NEWID(), @Name, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="txtTubeName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTubeDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlTubeActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsColour" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO ColourType VALUES (NEWID(), @Name, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="txtColourName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtColourDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlColourActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>