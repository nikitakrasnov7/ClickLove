package com.clicklove.gamegift;

import android.content.Context;
import android.net.Uri;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStream;

public class GameGiftImporter
{
    public static boolean copyUriToFile(
            Context context,
            String uriString,
            String destinationPath)
    {
        InputStream input = null;
        OutputStream output = null;

        try
        {
            Uri uri = Uri.parse(uriString);

            input = context
                    .getContentResolver()
                    .openInputStream(uri);

            if (input == null)
            {
                return false;
            }

            output = new FileOutputStream(destinationPath);

            byte[] buffer = new byte[81920];

            int length;

            while ((length = input.read(buffer)) != -1)
            {
                output.write(buffer, 0, length);
            }

            output.flush();

            return true;
        }
        catch (Exception e)
        {
            android.util.Log.e(
                    "GameGiftImporter",
                    "Ошибка копирования gamegift",
                    e
            );

            return false;
        }
        finally
        {
            try
            {
                if (input != null)
                    input.close();
            }
            catch (Exception ignored)
            {
            }

            try
            {
                if (output != null)
                    output.close();
            }
            catch (Exception ignored)
            {
            }
        }
    }
}