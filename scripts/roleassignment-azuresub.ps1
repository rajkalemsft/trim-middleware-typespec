
Install-Module ImportExcel -Scope CurrentUser

Connect-AzAccount

# Get the subscription context
$subscriptionId = ""
#Set-AzContext -SubscriptionId $subscriptionId


# Get a list of all Azure subscriptions associated with the account
$subscriptions = Get-AzSubscription

#All_Subscription_RoleAssignments
#All_SubscriptionResourceGroup_RoleAssignments
All_SubscriptionResource_RoleAssignments



function All_Subscription_RoleAssignments {
    
    $data = New-Object Collections.Generic.List[PSCustomObject]

    # Loop through each subscription and retrieve access details
    foreach ($subscription in $subscriptions) {
    
        #Select the current subscription
        if($subscription.Id -eq $subscriptionId)
        {
            Set-AzContext -Subscription $subscription.Id

            # Get a list of all role assignments in the subscription
            $roleAssignments = Get-AzRoleAssignment

            # Loop through each role assignment and display user and role details
            foreach ($roleAssignment in $roleAssignments) {

                $roleAssignmentDetails     = [PSCustomObject]@{
                    SubscriptioName        = $subscription.Name
                    SubscriptioId          = $subscription.Id
                    RoleName               = $roleAssignment.RoleDefinitionName
                    AssignedTo             = $roleAssignment.DisplayName
                    Scope                  = $roleAssignment.Scope
                    AssignedToObjectType   = $roleAssignment.ObjectType
                    AssignedToObjectId     = $roleAssignment.ObjectId
                }

                $data.Add($roleAssignmentDetails)
            }
        
        }
    }
    $data | Export-Excel -Path "C:\Raj-Scratch\AzureResourceSub_Access.xlsx" -AutoSize -AutoFilter -FreezeTopRow -BoldTopRow

}


function All_SubscriptionResourceGroup_RoleAssignments {
    $data = New-Object Collections.Generic.List[PSCustomObject]

    foreach ($subscription in $subscriptions) {
    
        #Select the current subscription
        if($subscription.Id -eq $subscriptionId)
        {
            Set-AzContext -Subscription $subscription.Id

            # Get a list of all resource groups in the subscription
            $resourceGroups = Get-AzResourceGroup

            # Loop through each resource group and get its access details
            foreach ($resourceGroup in $resourceGroups) {
    
                # Get a list of all role assignments in the resource group
                $roleAssignments = Get-AzRoleAssignment -ResourceGroupName $resourceGroup.ResourceGroupName
    
                # Loop through each role assignment and get its details
                foreach ($roleAssignment in $roleAssignments) {
                
                    $roleAssignmentDetails = [PSCustomObject]@{
                        SubscriptionName             = $subscription.Name
                        SubscriptionID               = $subscription.Id
                        ResourceGroupName            = $resourceGroup.ResourceGroupName
                        RoleName                     = $roleAssignment.RoleDefinitionName
                        AssignedToDisplayName        = $roleAssignment.DisplayName
                        Scope                        = $roleAssignment.Scope
                        AssignedToObjectType         = $roleAssignment.ObjectType
                        AssignedToObjectId           = $roleAssignment.ObjectId
                    }

                    $data.Add($roleAssignmentDetails)

                    }
                }
          }
    }
    $data | Export-Excel -Path "C:\Raj-Scratch\AzureResourceGroup_Access.xlsx" -AutoSize -AutoFilter -FreezeTopRow -BoldTopRow
}


function All_SubscriptionResource_RoleAssignments {
    $data = New-Object Collections.Generic.List[PSCustomObject]
    
    foreach ($subscription in $subscriptions) {
    
        #Select the current subscription
        if($subscription.Id -eq $subscriptionId)
        {
            Set-AzContext -Subscription $subscription.Id

            # Get all resources in the subscription
            $resources = Get-AzResource


            # Loop through each resource
            foreach ($resource in $resources) {

                    $roleAssignments = Get-AzRoleAssignment -Scope $resource.ResourceId

                    # Output the resource details and role assignments
                    $roleAssignmentDetails = [PSCustomObject]@{
                        ResourceName     = $resource.Name
                        ResourceType     = $resource.ResourceType
                    }

                    #Write-Output "Resource: $($resource.Name) [$($resource.ResourceType)]"
                    #Write-Output "Role Assignments:"
                    foreach ($assignment in $roleAssignments) {
                        if($assignment.PrincipalName)
                        {
                            #Write-Output "    $($assignment.RoleDefinitionName) - $($assignment.PrincipalName)"
                            $roleAssignmentDetails | Add-Member -MemberType NoteProperty -Name 'RoleName' -Value $assignment.RoleDefinitionName
                            $roleAssignmentDetails | Add-Member -MemberType NoteProperty -Name 'AssignedTo' -Value $assignment.PrincipalName

                        }
                        $data.Add($roleAssignmentDetails)
                    }
            }
        }

    }
    # Write data to Excel file
    $data | Export-Excel -Path "C:\Raj-Scratch\AzureResources_Access.xlsx" -AutoSize -AutoFilter -FreezeTopRow -BoldTopRow

}

