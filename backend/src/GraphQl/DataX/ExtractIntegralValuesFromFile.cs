

// public async Task<ExtractIntegralValuesFromFilesPayload> ExtractMirroredValuesFromFilesAsync(
//     ExtractIntegralValuesFromFilesInput input,
//     ApplicationDbContext context,
//     UserService userService,
//     ResponseApprovalService responseApprovalService,
//     CancellationToken cancellationToken
// )
// {
//     var currentUser = await userService.GetCurrentUser(cancellationToken);
//     if (currentUser is null)
//     {
//         return new ExtractIntegralValuesFromFilesPayload(
//             new ExtractIntegralValuesFromFilesError(
//                 ExtractIntegralValuesFromFilesErrorCode.UNAUTHENTICATED,
//                 $"The user is not authenticated.",
//                 []
//             )
//         );
//     }
//     if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
//     {
//         return new ExtractIntegralValuesFromFilesPayload(
//             new ExtractIntegralValuesFromFilesError(
//                 ExtractIntegralValuesFromFilesErrorCode.UNAUTHORIZED,
//                 $"The current user is not authorized to set mirrored values from files in this database.",
//                 []
//             )
//         );
//     }
// }