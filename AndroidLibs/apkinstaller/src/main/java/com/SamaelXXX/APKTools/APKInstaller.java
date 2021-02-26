package com.SamaelXXX.APKTools;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.os.Build;
import android.widget.Toast;

import java.io.File;
import java.util.List;

import android.support.v4.content.FileProvider;

public class APKInstaller {
    private Activity _unityActivity;

    Activity get_unityActivity()
    {
        if(null==_unityActivity)
        {
            try{
                Class<?> classtype=Class.forName("com.unity3d.player.UnityPlayer");
                Activity activity=(Activity)classtype.getDeclaredField("currentActivity").get(classtype);
                _unityActivity=activity;
            }
            catch (ClassNotFoundException e) {

            } catch (IllegalAccessException e) {

            } catch (NoSuchFieldException e) {

            }
        }
        return _unityActivity;
    }

    public boolean showToast(String content){
        Toast.makeText(get_unityActivity(),content,Toast.LENGTH_SHORT).show();
        return true;
    }

    public void InstallApk(String packageName, String apkPath) {
        File file = new File(apkPath);
        Intent intent = new Intent(Intent.ACTION_VIEW);
        Uri uri = null;
        try
        {
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            if(Build.VERSION.SDK_INT>=24)
            {
                intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
                uri = FileProvider.getUriForFile(get_unityActivity(), packageName+".fileprovider", file);
            }
            else
            {
                uri=Uri.fromFile(file);
            }

            intent.setDataAndType(uri, "application/vnd.android.package-archive");

            List<ResolveInfo> resolveLists = get_unityActivity().getPackageManager().queryIntentActivities(intent, PackageManager.MATCH_DEFAULT_ONLY);
            for (ResolveInfo resolveInfo : resolveLists)
            {
                String pkgName = resolveInfo.activityInfo.packageName;
                get_unityActivity().grantUriPermission(pkgName, uri, Intent.FLAG_GRANT_READ_URI_PERMISSION | Intent.FLAG_GRANT_WRITE_URI_PERMISSION);
            }
            get_unityActivity().startActivity(intent);
        }
        catch(IllegalArgumentException e)
        {
            showToast(e.getMessage());
            e.printStackTrace();
        }
    }
}
